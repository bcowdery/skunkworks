using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortAuthority.Web.Extensions
{
    public static class StringUriExtensions
    {

        /// <summary>
        /// Returns true if the string is a valid URI.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsValidUri(this string source)
        {
            bool result = Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult);

            if (result && uriResult != null)
            {
                result = uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps;
            }

            return result;
        }

        /// <summary>
        /// Gets the scheme plus authority of an Url (e.g. http://domain.com/index.html -> http://domain.com)
        /// </summary>
        public static string UriLeftPartAuthority(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            string trimSource = source.ToLowerInvariant().Replace(" ", string.Empty);  // remove spaces

            if (!trimSource.IsValidUri())
            {
                const string pattern = "://*.";

                if (trimSource.Length <= 14 || 
                    (!trimSource.StartsWith("http://") && !trimSource.StartsWith("https://")) || 
                    !trimSource.Contains(pattern))
                {
                    return string.Empty;
                }

                /* Wildcard based sub-domain pattern strings (http://*.xxx.com) not recognized as a valid uri.
                   The code block below tries to change it to a valid uri so as to get the right domain name 
                   string and then change it back with the wildcard
                */
                const string replacement = "://sub-domain.";
                var tempSource = trimSource.Replace(pattern, replacement);

                if (tempSource.IsValidUri())
                {
                    var leftAuthority = UriLeftPartAuthority(tempSource);

                    if (!string.IsNullOrEmpty(leftAuthority))  // success
                    {
                        return leftAuthority.Replace(replacement, pattern);  // restore
                    }
                }

                return string.Empty;

            }

            var uriAddress = new Uri(trimSource);
            return uriAddress.GetLeftPart(UriPartial.Authority);
        }
    }
}
