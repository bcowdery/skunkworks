using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shipyard.Web.Extensions;

namespace Shipyard.Web.Settings
{
    public class CorsSettings
    {
        /// <summary>
        /// Comma separated string of allowed origins
        /// </summary>
        public string AllowedOrigins { get; set; }

        /// <summary>
        /// Returns a list of unique allowed origins. Defaults to AllowAll ('*') if no origins set in the config file.
        /// </summary>
        /// <returns></returns>
        public string[] GetAllowedOrigins() 
        {
            var hosts = AllowedOrigins?.Split(',')
                .Select(x => x.UriLeftPartAuthority())  // scheme and authority
                .Where(x => !string.IsNullOrEmpty(x))   // filter out incorrect entries
                .ToArray();

            if (hosts == null || !hosts.Any())
            {
                hosts = new[] { "*" };
            }

            return hosts;
        }
    }
}
