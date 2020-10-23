using System;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Shipyard.Test
{
    /// <summary>
    /// Helper to load runtime configuration
    /// </summary>
    public static class ConfigurationHelper
    {
        private static IConfiguration _configuration;
        
        /// <summary>
        /// Returns the current configuration root.
        /// Lazily builds the configuration from the NUnit test context if not configured.
        /// </summary>
        public static IConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                BuildConfiguration(TestContext.CurrentContext, new ConfigurationBuilder());
            }

            return _configuration;
        }

        /// <summary>
        /// Builds a configuration root for an NUnit test context.
        /// </summary>
        /// <param name="testContext"></param>
        /// <param name="config"></param>
        public static void BuildConfiguration(TestContext testContext, IConfigurationBuilder config)
        {
            BuildConfiguration(testContext.TestDirectory, config);
        }

        /* AspNetCore */
        /*
        /// <summary>
        /// Builds a configuration root for an AspNetCore web host context.
        /// </summary>
        /// <param name="hostingContext"></param>
        /// <param name="config"></param>
        public static void BuildConfiguration(WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {
            BuildConfiguration(hostingContext.HostingEnvironment.ContentRootPath, config);
        }
        */

        private static void BuildConfiguration(string basePath, IConfigurationBuilder config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            config.SetBasePath(basePath);
            config.AddJsonFile("appsettings.json", optional: true);

            _configuration = config.Build();
        }
    }
}
