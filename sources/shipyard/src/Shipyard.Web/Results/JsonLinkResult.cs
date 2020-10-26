using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Shipyard.Web.Results
{
    /// <summary>
    /// Collection of links for JSON responses
    /// </summary>
    public class JsonLinks
    {
        public JsonLinks(IEnumerable<JsonLink> links)
        {
            Links = links as JsonLink[] ?? links?.ToArray();
        }

        [JsonPropertyName("_links")]
        public JsonLink[] Links { get; }

        /// <summary>
        /// Create a self referencing link in HATEOAS style.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static JsonLinks Self(string uri)
        {
            return new JsonLinks(new []
            {
                new JsonLink(uri, "self", "GET")
            });
        }
    }

    /// <summary>
    /// Reference link for JSON responses
    /// </summary>
    public class JsonLink
    {
        public JsonLink(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        public string Href { get; }
        public string Rel { get; }
        public string Method { get; }
    }
}
