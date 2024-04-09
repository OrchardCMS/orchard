using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Orchard.Services {
    public interface IHtmlFilterProcessor : IDependency {
        string ProcessFilters(string text, HtmlFilterContext context);
    }

    public static class HtmlFilterProcessorExtensions {
        public static string ProcessFilters(this IHtmlFilterProcessor processor, string text, string flavor, IDictionary<string, object> data) {
            return processor.ProcessFilters(text, new HtmlFilterContext { Flavor = flavor, Data = data });
        }

        public static string ProcessFilters(this IHtmlFilterProcessor processor, string text, string flavor) {
            return processor.ProcessFilters(text, new HtmlFilterContext { Flavor = flavor });
        }

        public static string ProcessFilters(this IHtmlFilterProcessor processor, string text, string flavor, IContent content) {
            return processor.ProcessFilters(text, new HtmlFilterContext { Flavor = flavor, Data = new Dictionary<string, object> { { "Content", content.ContentItem } } });
        }
    }
}