﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Feeds;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
using Orchard.Projections.Settings;
using Orchard.Projections.ViewModels;
using Orchard.Tokens;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Orchard.Projections.Drivers {
    public class ProjectionPartDriver : ContentPartDriver<ProjectionPart> {
        private readonly IOrchardServices _orchardServices;
        private readonly IRepository<QueryPartRecord> _queryRepository;
        private readonly IProjectionManager _projectionManager;
        private readonly IFeedManager _feedManager;
        private readonly ITokenizer _tokenizer;
        private readonly IDisplayHelperFactory _displayHelperFactory;
        private readonly IWorkContextAccessor _workContextAccessor;
        private const string TemplateName = "Parts/ProjectionPart";

        public ProjectionPartDriver(
            IOrchardServices orchardServices,
            IRepository<QueryPartRecord> queryRepository,
            IProjectionManager projectionManager,
            IFeedManager feedManager,
            ITokenizer tokenizer,
            IDisplayHelperFactory displayHelperFactory,
            IWorkContextAccessor workContextAccessor) {
            _orchardServices = orchardServices;
            _queryRepository = queryRepository;
            _projectionManager = projectionManager;
            _feedManager = feedManager;
            _tokenizer = tokenizer;
            _displayHelperFactory = displayHelperFactory;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix { get { return "ProjectionPart"; } }

        protected override DriverResult Display(ProjectionPart part, string displayType, dynamic shapeHelper) {
            var query = part.Record.QueryPartRecord;

            // retrieving paging parameters
            var queryString = _orchardServices.WorkContext.HttpContext.Request.QueryString;

            var pageKey = String.IsNullOrWhiteSpace(part.Record.PagerSuffix) ? "page" : "page-" + part.Record.PagerSuffix;
            var page = 0;

            // default page size
            int pageSize = part.Record.Items;

            // don't try to page if not necessary
            if (part.Record.DisplayPager && queryString.AllKeys.Contains(pageKey)) {
                Int32.TryParse(queryString[pageKey], out page);
            }

            // if 0, then assume "All", limit to 128 by default
            if (pageSize == 128) {
                pageSize = Int32.MaxValue;
            }

            // if pageSize is provided on the query string, ensure it is compatible with allowed limits
            var pageSizeKey = "pageSize" + part.Record.PagerSuffix;

            if (queryString.AllKeys.Contains(pageSizeKey)) {
                if (Int32.TryParse(queryString[pageSizeKey], out int qsPageSize)) {
                    if (part.Record.MaxItems == 0 || qsPageSize <= part.Record.MaxItems) {
                        pageSize = qsPageSize;
                    }
                }
            }

            var pager = new Pager(_orchardServices.WorkContext.CurrentSite, page, pageSize);

            var pagerShape = shapeHelper.Pager(pager)
                .ContentPart(part)
                .PagerId(pageKey);

            return Combined(
                ContentShape("Parts_ProjectionPart_Pager", shape => {
                    if (!part.Record.DisplayPager) {
                        return null;
                    }

                    return pagerShape;
                }),
                ContentShape("Parts_ProjectionPart_List", shape => {

                    // generates a link to the RSS feed for this term
                    var metaData = _orchardServices.ContentManager.GetItemMetadata(part.ContentItem);
                    _feedManager.Register(metaData.DisplayText, "rss", new RouteValueDictionary { { "projection", part.Id } });

                    // execute the query
                    var contentItems = _projectionManager.GetContentItems(query.Id, part, pager.GetStartIndex() + part.Record.Skip, pager.PageSize).ToList();

                    // sanity check so that content items with ProjectionPart can't be added here, or it will result in an infinite loop
                    contentItems = contentItems.Where(x => !x.Has<ProjectionPart>()).ToList();

                    // applying layout
                    var layout = part.Record.LayoutRecord;

                    LayoutDescriptor layoutDescriptor = layout == null ? null : _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).FirstOrDefault(x => x.Category == layout.Category && x.Type == layout.Type);

                    // create pager shape
                    if (part.Record.DisplayPager) {
                        var contentItemsCount = _projectionManager.GetCount(query.Id, part) - part.Record.Skip;
                        contentItemsCount = Math.Max(0, contentItemsCount);
                        pagerShape.TotalItemCount(contentItemsCount);
                    }

                    // renders in a standard List shape if no specific layout could be found
                    if (layoutDescriptor == null) {
                        var list = _orchardServices.New.List();
                        var contentShapes = contentItems.Select(item => _orchardServices.ContentManager.BuildDisplay(item, "Summary"));
                        list.AddRange(contentShapes);

                        return list;
                    }

                    var allFielDescriptors = _projectionManager.DescribeProperties().ToList();
                    var fieldDescriptors = layout.Properties.OrderBy(p => p.Position).Select(p => allFielDescriptors.SelectMany(x => x.Descriptors).Select(d => new { Descriptor = d, Property = p }).FirstOrDefault(x => x.Descriptor.Category == p.Category && x.Descriptor.Type == p.Type)).ToList();

                    var layoutComponents = contentItems.Select(
                        contentItem => {

                            var contentItemMetadata = _orchardServices.ContentManager.GetItemMetadata(contentItem);

                            var propertyDescriptors = fieldDescriptors.Select(
                                d => {
                                    var fieldContext = new PropertyContext {
                                        State = FormParametersHelper.ToDynamic(d.Property.State),
                                        Tokens = new Dictionary<string, object> { { "Content", contentItem } }
                                    };

                                    return new { d.Property, Shape = d.Descriptor.Property(fieldContext, contentItem) };
                                });

                            // apply all settings to the field content, wrapping it in a FieldWrapper shape
                            var properties = _orchardServices.New.Properties(
                                Items: propertyDescriptors.Select(
                                    pd => _orchardServices.New.PropertyWrapper(
                                        Item: pd.Shape,
                                        Property: pd.Property,
                                        ContentItem: contentItem,
                                        ContentItemMetadata: contentItemMetadata
                                    )
                                )
                            );

                            return new LayoutComponentResult {
                                ContentItem = contentItem,
                                ContentItemMetadata = contentItemMetadata,
                                Properties = properties
                            };

                        }).ToList();

                    var tokenizedState = _tokenizer.Replace(layout.State, new Dictionary<string, object> { { "Content", part.ContentItem } });

                    var renderLayoutContext = new LayoutContext {
                        State = FormParametersHelper.ToDynamic(tokenizedState),
                        LayoutRecord = layout,
                    };

                    if (layout.GroupProperty != null) {
                        var groupPropertyId = layout.GroupProperty.Id;
                        var display = _displayHelperFactory.CreateHelper(new ViewContext { HttpContext = _workContextAccessor.GetContext().HttpContext }, new ViewDataContainer());

                        // index by PropertyWrapper shape
                        var groups = layoutComponents.GroupBy(
                            x => {
                                var propertyShape = ((IEnumerable<dynamic>)x.Properties.Items).First(p => ((PropertyRecord)p.Property).Id == groupPropertyId);

                                // clear the wrappers, as they shouldn't be needed to generate the grouping key itself
                                // otherwise the DisplayContext.View is null, and throws an exception if a wrapper is rendered (#18558)
                                ((IShape)propertyShape).Metadata.Wrappers.Clear();

                                string key = Convert.ToString(display(propertyShape));
                                return key;
                            }).Select(x => new { Key = x.Key, Components = x });

                        var list = _orchardServices.New.List();
                        foreach (var group in groups) {

                            var localResult = layoutDescriptor.Render(renderLayoutContext, group.Components);
                            // add the Context to the shape
                            localResult.Context(renderLayoutContext);

                            list.Add(_orchardServices.New.LayoutGroup(Key: new MvcHtmlString(group.Key), List: localResult));
                        }

                        return list;
                    }


                    var layoutResult = layoutDescriptor.Render(renderLayoutContext, layoutComponents);

                    // add the Context to the shape
                    layoutResult.Context(renderLayoutContext);

                    return layoutResult;
                }));
        }

        protected override DriverResult Editor(ProjectionPart part, dynamic shapeHelper) {
            return ContentShape("Parts_ProjectionPart_Edit", () => {
                var model = new ProjectionPartEditViewModel();

                // for create read the setting values
                var settings = part.TypePartDefinition.Settings.GetModel<ProjectionPartSettings>();
                if (part.Id == 0) {
                    model = new ProjectionPartEditViewModel {
                        DisplayPager = settings.DisplayPager,
                        Items = settings.Items,
                        Skip = settings.Skip,
                        PagerSuffix = settings.PagerSuffix,
                        MaxItems = settings.MaxItems,
                        QueryLayoutRecordId = settings.QueryLayoutRecordId
                    };
                }
                else {
                    model = new ProjectionPartEditViewModel {
                        DisplayPager = part.Record.DisplayPager,
                        Items = part.Record.Items,
                        ItemsPerPage = part.Record.ItemsPerPage,
                        Skip = part.Record.Skip,
                        PagerSuffix = part.Record.PagerSuffix,
                        MaxItems = part.Record.MaxItems,
                        QueryLayoutRecordId = "-1"
                    };
                    // concatenated Query and Layout ids for the view
                    if (part.Record.QueryPartRecord != null) {
                        model.QueryLayoutRecordId = part.Record.QueryPartRecord.Id + ";";
                    }

                    if (part.Record.LayoutRecord != null) {
                        model.QueryLayoutRecordId += part.Record.LayoutRecord.Id.ToString();
                    }
                    else {
                        model.QueryLayoutRecordId += "-1";
                    }
                }

                model.PartId = part.Id;

                // lock fields
                model.LockEditingItems = settings.LockEditingItems;
                model.LockEditingSkip = settings.LockEditingSkip;
                model.LockEditingMaxItems = settings.LockEditingMaxItems;
                model.LockEditingPagerSuffix = settings.LockEditingPagerSuffix;
                model.LockEditingDisplayPager = settings.LockEditingDisplayPager;

                // populating the list of queries and layouts
                var layouts = _projectionManager.DescribeLayouts().SelectMany(x => x.Descriptors).ToList();
                model.QueryRecordEntries = _orchardServices.ContentManager.Query<QueryPart, QueryPartRecord>().Join<TitlePartRecord>().OrderBy(x => x.Title).List()
                        .Select(x => new QueryRecordEntry {
                            Id = x.Id,
                            Name = x.Name,
                            LayoutRecordEntries = x.Layouts.Select(l => new LayoutRecordEntry {
                                Id = l.Id,
                                Description = GetLayoutDescription(layouts, l)
                            })
                        });

                // if any values, use default list of the settings
                if (!string.IsNullOrWhiteSpace(settings.FilterQueryRecordId)) {
                    var filterQueryRecordId = settings.FilterQueryRecordId.Split('&');
                    model.QueryRecordIdFilterEntries = filterQueryRecordId
                        .Select(x => new QueryRecordFilterEntry {
                            Id = x.Split(';')[0],
                            LayoutId = x.Split(';')[1]
                        });
                }
                else {
                    model.QueryRecordIdFilterEntries = new List<QueryRecordFilterEntry>();
                }

                return shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: Prefix);
            });
        }

        private static string GetLayoutDescription(IEnumerable<LayoutDescriptor> layouts, LayoutRecord l) {
            var descriptor = layouts.FirstOrDefault(x => l.Category == x.Category && l.Type == x.Type);
            return String.IsNullOrWhiteSpace(l.Description) ? descriptor.Display(new LayoutContext { State = FormParametersHelper.ToDynamic(l.State) }).Text : l.Description;
        }

        protected override DriverResult Editor(ProjectionPart part, IUpdateModel updater, dynamic shapeHelper) {
            var settings = part.TypePartDefinition.Settings.GetModel<ProjectionPartSettings>();

            var model = new ProjectionPartEditViewModel();

            updater.TryUpdateModel(model, Prefix, null, null);

            model.PartId = part.Id;

            // check the setting, if it is unlocked, assign the setting value
            if (settings.LockEditingDisplayPager) {
                part.Record.DisplayPager = settings.DisplayPager;
            }
            else {
                part.Record.DisplayPager = model.DisplayPager;
            }
            if (settings.LockEditingItems) {
                part.Record.Items = settings.Items;
            }
            else {
                part.Record.Items = model.Items;
            }
            part.Record.ItemsPerPage = model.ItemsPerPage;
            if (settings.LockEditingSkip) {
                part.Record.Skip = settings.Skip;
            }
            else {
                part.Record.Skip = model.Skip;
            }
            if (settings.LockEditingMaxItems) {
                part.Record.MaxItems = settings.MaxItems;
            }
            else {
                part.Record.MaxItems = model.MaxItems;
            }
            if (settings.LockEditingPagerSuffix) {
                part.Record.PagerSuffix = (settings.PagerSuffix ?? String.Empty).Trim();
            }
            else {
                part.Record.PagerSuffix = (model.PagerSuffix ?? String.Empty).Trim();
            }

            var queryLayoutIds = model.QueryLayoutRecordId.Split(new[] { ';' });

            part.Record.QueryPartRecord = _queryRepository.Get(Int32.Parse(queryLayoutIds[0]));
            part.Record.LayoutRecord = part.Record.QueryPartRecord.Layouts.FirstOrDefault(x => x.Id == Int32.Parse(queryLayoutIds[1]));

            if (!String.IsNullOrWhiteSpace(part.Record.PagerSuffix) && !String.Equals(part.Record.PagerSuffix.ToSafeName(), part.Record.PagerSuffix, StringComparison.OrdinalIgnoreCase)) {
                updater.AddModelError("PagerSuffix", T("Suffix should not contain special characters."));
            }

            if (model.Items == 0
                && (part.Record.QueryPartRecord?.FilterGroups.Any(group => group.Filters.Count == 0) ?? false)) {
                _orchardServices.Notifier.Warning(
                    T("The selected Query has at least one empty filter group, which causes all content items to be returned. It is recommended to limit the number of content items queried by setting the 'Items to display' field to a non-zero value."));
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(ProjectionPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            context.ImportAttribute(part.PartDefinition.Name, "Items", x => part.Record.Items = Int32.Parse(x));
            context.ImportAttribute(part.PartDefinition.Name, "ItemsPerPage", x => part.Record.ItemsPerPage = Int32.Parse(x));
            context.ImportAttribute(part.PartDefinition.Name, "Offset", x => part.Record.Skip = Int32.Parse(x));
            context.ImportAttribute(part.PartDefinition.Name, "PagerSuffix", x => part.Record.PagerSuffix = x);
            context.ImportAttribute(part.PartDefinition.Name, "MaxItems", x => part.Record.MaxItems = Int32.Parse(x));
            context.ImportAttribute(part.PartDefinition.Name, "DisplayPager", x => part.Record.DisplayPager = Boolean.Parse(x));
        }

        protected override void ImportCompleted(ProjectionPart part, ImportContentContext context) {
            // Assign the query only when everything is imported.
            var query = context.Attribute(part.PartDefinition.Name, "Query");
            if (query != null && context.GetItemFromSession(query).As<QueryPart>() != null) {
                part.Record.QueryPartRecord = context.GetItemFromSession(query).As<QueryPart>().Record;
                var layoutIndex = context.Attribute(part.PartDefinition.Name, "LayoutIndex");
                if (layoutIndex != null
                    && Int32.TryParse(layoutIndex, out int layoutIndexValue)
                    && layoutIndexValue >= 0
                    && part.Record.QueryPartRecord.Layouts.Count > layoutIndexValue) {
                    part.Record.LayoutRecord = part.Record.QueryPartRecord.Layouts[Int32.Parse(layoutIndex)];
                }
            }
        }

        protected override void Exporting(ProjectionPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Items", part.Record.Items);
            context.Element(part.PartDefinition.Name).SetAttributeValue("ItemsPerPage", part.Record.ItemsPerPage);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Offset", part.Record.Skip);
            context.Element(part.PartDefinition.Name).SetAttributeValue("PagerSuffix", part.Record.PagerSuffix);
            context.Element(part.PartDefinition.Name).SetAttributeValue("MaxItems", part.Record.MaxItems);
            context.Element(part.PartDefinition.Name).SetAttributeValue("DisplayPager", part.Record.DisplayPager);

            if (part.Record.QueryPartRecord != null) {
                var queryPart = _orchardServices.ContentManager.Query<QueryPart, QueryPartRecord>("Query").Where(x => x.Id == part.Record.QueryPartRecord.Id).List().FirstOrDefault();
                if (queryPart != null) {
                    var queryIdentity = _orchardServices.ContentManager.GetItemMetadata(queryPart).Identity;
                    context.Element(part.PartDefinition.Name).SetAttributeValue("Query", queryIdentity.ToString());
                    context.Element(part.PartDefinition.Name).SetAttributeValue("LayoutIndex", part.Record.QueryPartRecord.Layouts.IndexOf(part.Record.LayoutRecord));
                }
            }
        }

        protected override void Cloning(ProjectionPart originalPart, ProjectionPart clonePart, CloneContentContext context) {
            clonePart.Record.Items = originalPart.Record.Items;
            clonePart.Record.ItemsPerPage = originalPart.Record.ItemsPerPage;
            clonePart.Record.Skip = originalPart.Record.Skip;
            clonePart.Record.PagerSuffix = originalPart.Record.PagerSuffix;
            clonePart.Record.MaxItems = originalPart.Record.MaxItems;
            clonePart.Record.DisplayPager = originalPart.Record.DisplayPager;
            clonePart.Record.QueryPartRecord = originalPart.Record.QueryPartRecord;
            clonePart.Record.LayoutRecord = originalPart.Record.LayoutRecord;
        }

        private class ViewDataContainer : IViewDataContainer {
            public ViewDataDictionary ViewData { get; set; }
        }
    }
}

