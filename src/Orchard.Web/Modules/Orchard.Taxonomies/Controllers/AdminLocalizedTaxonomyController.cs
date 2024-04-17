﻿using System.Web.Routing;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using Orchard.Taxonomies.Services;
using Orchard.UI.Admin;

namespace Orchard.Taxonomies.Controllers {
    [OrchardFeature("Orchard.Taxonomies.LocalizationExtensions")]
    public class AdminLocalizedTaxonomyController : LocalizedTaxonomyController {
        private readonly RequestContext _requestContext;

        public AdminLocalizedTaxonomyController(IContentDefinitionManager contentDefinitionManager,
            ILocalizationService localizationService,
            ITaxonomyService taxonomyService,
            ITaxonomyExtensionsService
            taxonomyExtensionsService,
            RequestContext requestContext) : base(contentDefinitionManager,
                localizationService,
                taxonomyService,
                taxonomyExtensionsService) {

            _requestContext = requestContext;
        }

        protected override void ApplyPreRequest() {
            AdminFilter.Apply(_requestContext);
        }
    }
}
