using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;


namespace Shipyard.Web.Extensions
{
    public static class ApplicationModelConventionExtensions
    {
        /// <summary>
        /// Use lowercase, hyphen separated route tokens instead of CapitalCase.
        /// </summary>
        /// <param name="conventions"></param>
        public static void UseSlugifiedRoutes(this IList<IApplicationModelConvention> conventions)
        {
            conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        }
    }

    /// <summary>
    /// Slugifies parameter names converts capital case strings to lowercase hyphenated
    /// (e.g., 'CapitalCase' -> 'capital-case').
    /// </summary>
    public class SlugifyParameterTransformer: IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            return value == null
                ? null
                : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        } 
    }
}
