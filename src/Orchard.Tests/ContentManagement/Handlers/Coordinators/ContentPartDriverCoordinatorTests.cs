﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Moq;
using NUnit.Framework;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Drivers.Coordinators;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Implementation;

namespace Orchard.Tests.ContentManagement.Handlers.Coordinators {
    [TestFixture]
    public class ContentPartDriverCoordinatorTests {
        private IContainer _container;

        [SetUp]
        public void Init() {
            var builder = new ContainerBuilder();
            //builder.RegisterModule(new ImplicitCollectionSupportModule());
            builder.RegisterType<ContentPartDriverCoordinator>().As<IContentHandler>();
            builder.RegisterType<DefaultShapeFactory>().As<IShapeFactory>();
            builder.RegisterInstance(new Mock<IContentDefinitionManager>().Object);
            _container = builder.Build();
        }

        [Test]
        public async Task  DriverHandlerAsyncShouldNotThrowException() {
            var contentHandler = _container.Resolve<IContentHandler>();
            await contentHandler.BuildDisplayAsync(null);
        }

        [Test]
        public async Task AllAsyncDriversShouldBeCalled() {
            var driver1 = new Mock<IContentPartDriver>();
            var driver2 = new Mock<IContentPartDriver>();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver1.Object);
            builder.RegisterInstance(driver2.Object);
            builder.Update(_container);
            var contentHandler = _container.Resolve<IContentHandler>();

            var contentItem = new ContentItem();
            var context = new BuildDisplayContext(null, contentItem, "", "", new Mock<IShapeFactory>().Object);

            driver1.Verify(x => x.BuildDisplayAsync(context), Times.Never());
            driver2.Verify(x => x.BuildDisplayAsync(context), Times.Never());
            await contentHandler.BuildDisplayAsync(context);
            driver1.Verify(x => x.BuildDisplayAsync(context));
            driver2.Verify(x => x.BuildDisplayAsync(context));
        }

        [Test]
        public async Task AsyncDriversAreAppliedInOrderOfExecution() {
            var driver1 = new Mock<IContentPartDriver>();
            var driver2 = new Mock<IContentPartDriver>();
            var result1 = new Mock<DriverResult>();
            var result2 = new Mock<DriverResult>();
            long executed1 = 0;
            long executed2 = 0;
            driver1.Setup(d => d.BuildDisplayAsync(It.IsAny<BuildDisplayContext>())).Returns(async () => {
                await Task.Delay(50);
                return result1.Object;
            });
            driver2.Setup(d => d.BuildDisplayAsync(It.IsAny<BuildDisplayContext>())).Returns(async () => {
                await Task.Delay(1);
                return result2.Object;
            });
            result1.Setup(r => r.ApplyAsync(It.IsAny<BuildDisplayContext>())).Returns(Task.Delay(0)).Callback(() => executed1 = DateTime.Now.Ticks);
            result2.Setup(r => r.ApplyAsync(It.IsAny<BuildDisplayContext>())).Returns(Task.Delay(0)).Callback(() => executed2 = DateTime.Now.Ticks);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver1.Object);
            builder.RegisterInstance(driver2.Object);
            builder.Update(_container);
            var contentHandler = _container.Resolve<IContentHandler>();

            var contentItem = new ContentItem();
            var context = new BuildDisplayContext(null, contentItem, "", "", new Mock<IShapeFactory>().Object);

            await contentHandler.BuildDisplayAsync(context);

            Assert.LessOrEqual(executed2, executed1);
        }

        [Test]
        public async Task AsyncDriversAreAsync() {
            var driver1 = new Mock<IContentPartDriver>();
            var driver2 = new Mock<IContentPartDriver>();
            driver1.Setup(d => d.BuildDisplayAsync(It.IsAny<BuildDisplayContext>())).Returns(async () => {
                await Task.Delay(30);
                return null;
            });
            driver2.Setup(d => d.BuildDisplayAsync(It.IsAny<BuildDisplayContext>())).Returns(async () => {
                await Task.Delay(30);
                return null;
            });
            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver1.Object);
            builder.RegisterInstance(driver2.Object);
            builder.Update(_container);
            var contentHandler = _container.Resolve<IContentHandler>();

            var contentItem = new ContentItem();
            var context = new BuildDisplayContext(null, contentItem, "", "", new Mock<IShapeFactory>().Object);

            var watch = new Stopwatch();
            watch.Start();
            await contentHandler.BuildDisplayAsync(context);
            watch.Stop();
            Assert.GreaterOrEqual(watch.ElapsedMilliseconds, 30);
            Assert.LessOrEqual(watch.ElapsedMilliseconds, 60);
        }

        [Test, Ignore("no implementation for IZoneCollection")]
        public async Task TestDriverCanAddDisplay() {
            var driver = new StubPartDriver();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(driver).As<IContentPartDriver>();
            builder.Update(_container);
            var contentHandler = _container.Resolve<IContentHandler>();
            dynamic shapeFactory = _container.Resolve<IShapeFactory>();

            var contentItem = new ContentItem();
            contentItem.Weld(new StubPart { Foo = new[] { "a", "b", "c" } });

            var ctx = new BuildDisplayContext(null, null, "", "", null);
            var context = shapeFactory.Context(ctx);
            Assert.That(context.TopMeta, Is.Null);
            await contentHandler.BuildDisplayAsync(ctx);
            Assert.That(context.TopMeta, Is.Not.Null);
            Assert.That(context.TopMeta.Count == 1);
        }

        public class StubPartDriver : ContentPartDriver<StubPart> {
            protected override string Prefix {
                get { return "Stub"; }
            }

            protected override DriverResult Display(StubPart part, string displayType, dynamic shapeHelper) {
                var stub = shapeHelper.Stub(Foo: string.Join(",", part.Foo));
                if (!string.IsNullOrWhiteSpace(displayType))
                    stub.Metadata.Type = string.Format("{0}.{1}", stub.Metadata.Type, displayType);
                return ContentShape(stub).Location("TopMeta");
                
                //var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                //if (displayType.StartsWith("Summary"))
                //    return ContentPartTemplate(viewModel, "StubViewModelTerse").Location("TopMeta");

                //return ContentPartTemplate(viewModel).Location("TopMeta");
            }

            protected override DriverResult Editor(StubPart part, dynamic shapeHelper) {
                var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                return new ContentTemplateResult(viewModel, null, Prefix).Location("last", "10");
            }

            protected override DriverResult Editor(StubPart part, IUpdateModel updater, dynamic shapeHelper) {
                var viewModel = new StubViewModel { Foo = string.Join(",", part.Foo) };
                updater.TryUpdateModel(viewModel, Prefix, null, null);
                part.Foo = viewModel.Foo.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();
                return new ContentTemplateResult(viewModel, null, Prefix).Location("last", "10");
            }
        }

        public class StubPart : ContentPart {
            public string[] Foo { get; set; }
        }

        public class StubViewModel {
            [Required]
            public string Foo { get; set; }
        }
    }
}
