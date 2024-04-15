using System.Collections.Generic;
using System.Linq;
using Orchard.Autoroute.Models;
using Orchard.Autoroute.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Localization.Models;

namespace Orchard.Localization.Services {
    public class LocalizationService : ILocalizationService {
        private readonly IContentManager _contentManager;
        private readonly ICultureManager _cultureManager;
        private readonly IHomeAliasService _homeAliasService;

        public LocalizationService(IContentManager contentManager, ICultureManager cultureManager, IHomeAliasService homeAliasService) {
            _contentManager = contentManager;
            _cultureManager = cultureManager;
            _homeAliasService = homeAliasService;
        }


        public LocalizationPart GetLocalizedContentItem(IContent content, string culture) {
            // Warning: Returns only the first of same culture localizations.
            return GetLocalizedContentItem(content, culture, null);
        }

        public LocalizationPart GetLocalizedContentItem(IContent content, string culture, VersionOptions versionOptions) {
            var cultureRecord = _cultureManager.GetCultureByName(culture);

            if (cultureRecord == null) return null;

            var localized = content.As<LocalizationPart>();

            if (localized == null) return null;

            var masterContentItemId = localized.HasTranslationGroup ? localized.Record.MasterContentItemId : localized.Id;

            // Warning: Returns only the first of same culture localizations.
            return _contentManager
                .Query<LocalizationPart>(versionOptions, content.ContentItem.ContentType)
                .Where<LocalizationPartRecord>(l =>
                    (l.Id == masterContentItemId || l.MasterContentItemId == masterContentItemId) &&
                    l.CultureId == cultureRecord.Id)
                .Slice(1)
                .FirstOrDefault();
        }

        public string GetContentCulture(IContent content) {
            var localized = content.As<LocalizationPart>();

            return localized?.Culture == null ?
                _cultureManager.GetSiteCulture() :
                localized.Culture.Culture;
        }

        public void SetContentCulture(IContent content, string culture) {
            var localized = content.As<LocalizationPart>();

            if (localized == null) return;

            localized.Culture = _cultureManager.GetCultureByName(culture);
        }

        public IEnumerable<LocalizationPart> GetLocalizations(IContent content) {
            // Warning: May contain more than one localization of the same culture.
            return GetLocalizations(content, null);
        }

        public IEnumerable<LocalizationPart> GetLocalizations(IContent content, VersionOptions versionOptions) {
            if (content.ContentItem.Id == 0) return Enumerable.Empty<LocalizationPart>();

            var localized = content.As<LocalizationPart>();

            var query = versionOptions == null ?
                _contentManager.Query<LocalizationPart>(localized.ContentItem.ContentType) :
                _contentManager.Query<LocalizationPart>(versionOptions, localized.ContentItem.ContentType);

            int contentItemId = localized.ContentItem.Id;

            if (localized.HasTranslationGroup) {
                int masterContentItemId = localized.MasterContentItem.ContentItem.Id;

                query = query.Where<LocalizationPartRecord>(l =>
                    l.Id != contentItemId && // Exclude the content
                    (l.Id == masterContentItemId || l.MasterContentItemId == masterContentItemId));
            }
            else {
                query = query.Where<LocalizationPartRecord>(l => l.MasterContentItemId == contentItemId);
            }

            // Warning: May contain more than one localization of the same culture.
            return query.List().ToList();
        }

        bool ILocalizationService.TryGetRouteForUrl(string url, out AutoroutePart route) {
            route = _contentManager.Query<AutoroutePart, AutoroutePartRecord>()
                .ForVersion(VersionOptions.Published)
                .Where(r => r.DisplayAlias == url)
                .List()
                .FirstOrDefault();

            if (route == null)
                route = _homeAliasService.GetHomePage(VersionOptions.Latest).As<AutoroutePart>();
            return route != null;
        }

        bool ILocalizationService.TryFindLocalizedRoute(ContentItem routableContent, string cultureName, out AutoroutePart localizedRoute) {
            if (!routableContent.Parts.Any(p => p.Is<ILocalizableAspect>())) {
                localizedRoute = null;
                return false;
            }

            IEnumerable<LocalizationPart> localizations = ((ILocalizationService)this).GetLocalizations(routableContent, VersionOptions.Published);

            ILocalizableAspect localizationPart = null, siteCultureLocalizationPart = null;
            foreach (LocalizationPart l in localizations) {
                if (l.Culture.Culture == cultureName) {
                    localizationPart = l;
                    break;
                }
                if (l.Culture == null && siteCultureLocalizationPart == null) {
                    siteCultureLocalizationPart = l;
                }
            }

            if (localizationPart == null) {
                localizationPart = siteCultureLocalizationPart;
            }

            if (localizationPart == null) {
                localizedRoute = null;
                return false;
            }

            ContentItem localizedContentItem = localizationPart.ContentItem;
            localizedRoute = localizedContentItem.Parts.Single(p => p is AutoroutePart).As<AutoroutePart>();
            return true;
        }

    }
}
