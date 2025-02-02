﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Taxonomies.Helpers;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.Services;
using Orchard.UI.Navigation;

namespace Orchard.Taxonomies.Navigation {
    /// <summary>
    /// Dynamically injects taxonomy items as menu items on TaxonomyNavigationMenuItem elements
    /// </summary>
    public class TaxonomyNavigationProvider : INavigationFilter {
        private readonly IContentManager _contentManager;
        private readonly ITaxonomyService _taxonomyService;

        public TaxonomyNavigationProvider(
            IContentManager contentManager,
            ITaxonomyService taxonomyService) {
            _contentManager = contentManager;
            _taxonomyService = taxonomyService;

            _termsMemory = new Dictionary<string, TermPart[]>();
        }

        // Prevent doing the same query for terms more than once on a same request
        // in case we are building menus from the same starting taxonomies. Key is a
        // string to "combine" the Id of the root TermPart and the flag telling to
        // add that root to the results.
        private Dictionary<string, TermPart[]> _termsMemory;

        public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> items) {

            foreach (var item in items) {
                if (item.Content != null && item.Content.ContentItem.ContentType == "TaxonomyNavigationMenuItem") {

                    var taxonomyNavigationPart = item.Content.As<TaxonomyNavigationPart>();

                    var rootTerm = _taxonomyService.GetTerm(taxonomyNavigationPart.TermId);

                    TermPart[] allTerms;
                    string memoryKey;
                    if (rootTerm != null) {
                        memoryKey = $"{rootTerm.Id}_{taxonomyNavigationPart.DisplayRootTerm}";
                        if (!_termsMemory.ContainsKey(memoryKey)) {
                            // if DisplayRootTerm is specified add it to the menu items to render
                            _termsMemory[memoryKey] = _taxonomyService.GetChildren(rootTerm, taxonomyNavigationPart.DisplayRootTerm).ToArray();
                        }
                    }
                    else {
                        memoryKey = taxonomyNavigationPart.TaxonomyId.ToString();
                        if (!_termsMemory.ContainsKey(memoryKey)) {
                            _termsMemory[memoryKey] = _taxonomyService.GetTerms(taxonomyNavigationPart.TaxonomyId).ToArray();
                        }
                    }
                    allTerms = _termsMemory[memoryKey];

                    var rootLevel = rootTerm != null
                        ? rootTerm.GetLevels()
                        : 0;

                    var menuPosition = item.Position;
                    var rootPath = rootTerm == null || taxonomyNavigationPart.DisplayRootTerm ? "" : rootTerm.FullPath;

                    var startLevel = rootLevel + 1;
                    if (rootTerm == null || taxonomyNavigationPart.DisplayRootTerm) {
                        startLevel = rootLevel;
                    }

                    var endLevel = Int32.MaxValue;
                    if (taxonomyNavigationPart.LevelsToDisplay > 0) {
                        endLevel = startLevel + taxonomyNavigationPart.LevelsToDisplay - 1;
                    }

                    foreach (var contentItem in allTerms) {
                        if (contentItem != null) {
                            var part = contentItem;
                            var level = part.GetLevels();

                            // filter levels ?
                            if (level < startLevel || level > endLevel) {
                                continue;
                            }

                            // ignore menu item if there are no content items associated to the term
                            if (taxonomyNavigationPart.HideEmptyTerms && part.Count == 0) {
                                continue;
                            }

                            var menuText = _contentManager.GetItemMetadata(part).DisplayText;
                            var routes = _contentManager.GetItemMetadata(part).DisplayRouteValues;

                            if (taxonomyNavigationPart.DisplayContentCount) {
                                menuText += " (" + part.Count + ")";
                            }

                            // create 
                            var positions = contentItem.FullPath.Substring(rootPath.Length)
                                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => Array.FindIndex(allTerms, t => t.Id == Int32.Parse(p)))
                                .ToArray();

                            var inserted = new MenuItem {
                                Text = new LocalizedString(menuText),
                                IdHint = item.IdHint,
                                Classes = item.Classes,
                                Url = item.Url,
                                Href = item.Href,
                                LinkToFirstChild = false,
                                RouteValues = routes,
                                LocalNav = item.LocalNav,
                                Items = new MenuItem[0],
                                Position = menuPosition + ":" + String.Join(".", positions.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()),
                                Permissions = item.Permissions,
                                Content = part
                            };

                            yield return inserted;
                        }
                    }
                }
                else {
                    yield return item;
                }
            }
        }
    }
}