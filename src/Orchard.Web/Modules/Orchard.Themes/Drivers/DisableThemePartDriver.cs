﻿using System.Web;
using Orchard.ContentManagement.Drivers;
using Orchard.Themes.Models;

namespace Orchard.Themes.Drivers {
    public class DisableThemePartDriver : ContentPartDriver<DisableThemePart> {
        private readonly HttpContextBase _httpContext;

        public DisableThemePartDriver(HttpContextBase httpContext) {
            _httpContext = httpContext;
        }

        protected override DriverResult Display(DisableThemePart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_DisableTheme", () => {
                ThemeFilter.Disable(_httpContext.Request.RequestContext);
                return null;
            });
        }
    }

}