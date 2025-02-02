﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Feeds;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.Taxonomies.Settings;
using Orchard.UI.Navigation;
using System.Text;

namespace Orchard.Taxonomies.Drivers {
    public class TermPartDriver : ContentPartDriver<TermPart> {
        private readonly ITaxonomyService _taxonomyService;
        private readonly ISiteService _siteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeedManager _feedManager;
        private readonly IContentManager _contentManager;

        public TermPartDriver(
            ITaxonomyService taxonomyService,
            ISiteService siteService,
            IHttpContextAccessor httpContextAccessor,
            IFeedManager feedManager,
            IContentManager contentManager) {
            _taxonomyService = taxonomyService;
            _siteService = siteService;
            _httpContextAccessor = httpContextAccessor;
            _feedManager = feedManager;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        protected override string Prefix { get { return "Term"; } }

        protected override DriverResult Display(TermPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_TermPart_Feed", () => {

                    // generates a link to the RSS feed for this term
                    _feedManager.Register(part.Name, "rss", new RouteValueDictionary { { "term", part.Id } });
                    return null;
                }),
                ContentShape("Parts_TermPart", () => {
                    var pagerParameters = new PagerParameters();
                    var httpContext = _httpContextAccessor.Current();
                    if (httpContext != null) {
                        // Check if "page" parameter is a valid number.
                        int page = 0;
                        if (int.TryParse(httpContext.Request.QueryString["page"], out page)) {
                            pagerParameters.Page = page;
                        }
                    }

                    var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
                    var taxonomy = _taxonomyService.GetTaxonomy(part.TaxonomyId);
                    var totalItemCount = _taxonomyService.GetContentItemsCount(part);

                    var partSettings = part.Settings.GetModel<TermPartSettings>();
                    if (partSettings != null && partSettings.OverrideDefaultPagination) {
                        pager.PageSize = partSettings.PageSize;
                    }

                    var childDisplayType = partSettings != null &&
                                           !String.IsNullOrWhiteSpace(partSettings.ChildDisplayType)
                        ? partSettings.ChildDisplayType
                        : "Summary";
                    // asign Taxonomy and Term to the content item shape (Content) in order to provide 
                    // alternates when those content items are displayed when they are listed on a term
                    var termContentItems = _taxonomyService.GetContentItems(part, pager.GetStartIndex(), pager.PageSize)
                        .Select(c => _contentManager.BuildDisplay(c, childDisplayType).Taxonomy(taxonomy).Term(part));

                    var list = shapeHelper.List();

                    list.AddRange(termContentItems);

                    var pagerShape = pager.PageSize == 0
                        ? null
                        : shapeHelper.Pager(pager)
                            .TotalItemCount(totalItemCount)
                            .Taxonomy(taxonomy)
                            .Term(part);

                    return shapeHelper.Parts_TermPart(ContentItems: list, Taxonomy: taxonomy, Pager: pagerShape);
                }));
        }

        protected override DriverResult Editor(TermPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Taxonomies_Term_Fields",
                    () => shapeHelper.EditorTemplate(TemplateName: "Parts/Taxonomies.Term.Fields", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(TermPart termPart, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(termPart, Prefix, null, null);
            return Editor(termPart, shapeHelper);
        }

        protected override void Exporting(TermPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Count", part.Count);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Selectable", part.Selectable);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Weight);
            context.Element(part.PartDefinition.Name).SetAttributeValue("FullWeight", part.FullWeight);

            var taxonomy = _contentManager.Get(part.TaxonomyId);
            var identity = _contentManager.GetItemMetadata(taxonomy).Identity.ToString();
            context.Element(part.PartDefinition.Name).SetAttributeValue("TaxonomyId", identity);

            var identityPaths = new List<string>();
            foreach (var pathPart in part.Path.Split('/')) {
                if (String.IsNullOrEmpty(pathPart)) {
                    continue;
                }

                var parent = _contentManager.Get(Int32.Parse(pathPart));
                identityPaths.Add(_contentManager.GetItemMetadata(parent).Identity.ToString());
            }

            context.Element(part.PartDefinition.Name).SetAttributeValue("Path", String.Join(",", identityPaths.ToArray()));
        }

        protected override void Importing(TermPart part, ImportContentContext context) {
            // Don't do anything if the tag is not specified.
            if (context.Data.Element(part.PartDefinition.Name) == null) {
                return;
            }

            part.Count = Int32.Parse(context.Attribute(part.PartDefinition.Name, "Count"));
            part.Selectable = Boolean.Parse(context.Attribute(part.PartDefinition.Name, "Selectable"));
            part.Weight = Int32.Parse(context.Attribute(part.PartDefinition.Name, "Weight"));
            context.ImportAttribute(part.PartDefinition.Name, "FullWeight", s => part.FullWeight = s);
            bool createFullWeigth = string.IsNullOrWhiteSpace(part.FullWeight);

            var identity = context.Attribute(part.PartDefinition.Name, "TaxonomyId");
            var contentItem = context.GetItemFromSession(identity);

            if (contentItem == null) {
                throw new OrchardException(T("Unknown taxonomy: {0}", identity));
            }

            part.TaxonomyId = contentItem.Id;
            part.Path = "/";

            foreach (var identityPath in context.Attribute(part.PartDefinition.Name, "Path").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                var pathContentItem = context.GetItemFromSession(identityPath);
                part.Path += pathContentItem.Id + "/";
                if (createFullWeigth) {
                    part.FullWeight = _taxonomyService.ComputeFullWeight(part);
                }
            }
            if (createFullWeigth) {
                part.FullWeight = _taxonomyService.ComputeFullWeight(part);
            }
        }

        protected override void Cloning(TermPart originalPart, TermPart clonePart, CloneContentContext context) {
            // Arguably, copying the container/parent should be done elsewhere,
            // but since it is required to be certain of its value for proper use
            // of the TermPart we also do it here.
            clonePart.Selectable = originalPart.Selectable;
            clonePart.Weight = originalPart.Weight;
            clonePart.TaxonomyId = originalPart.TaxonomyId;
            clonePart.Container = originalPart.Container;
            _taxonomyService.ProcessPath(clonePart);
        }
    }
}