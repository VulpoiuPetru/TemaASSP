using Microsoft.Extensions.Configuration;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DomainModel;

namespace ServiceLayer
{
    /// <summary>
    /// Service for managing library configuration and calculating reader-specific limits
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly LibraryConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the ConfigurationService class
        /// Reads configuration from app.config/web.config
        /// </summary>
        public ConfigurationService()
        {
            _config = new LibraryConfiguration
            {
                DOMENII = GetConfigValue("DOMENII", 5),
                NMC = GetConfigValue("NMC", 10),
                PER = GetConfigValue("PER", 30),
                C = GetConfigValue("C", 5),
                D = GetConfigValue("D", 3),
                L = GetConfigValue("L", 6),
                LIM = GetConfigValue("LIM", 15),
                DELTA = GetConfigValue("DELTA", 14),
                NCZ = GetConfigValue("NCZ", 3),
                PERSIMP = GetConfigValue("PERSIMP", 20)
            };
        }

        /// <summary>
        /// Reads an integer value from app.config with a default fallback
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if key not found or invalid</param>
        /// <returns>Configuration value or default</returns>
        private int GetConfigValue(string key, int defaultValue)
        {
            try
            {
                var value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(value))
                    return defaultValue;

                return int.TryParse(value, out var result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the library configuration settings
        /// </summary>
        /// <returns>Library configuration</returns>
        public LibraryConfiguration GetConfiguration()
        {
            return _config;
        }

        /// <summary>
        /// Get reader-specific limits (adjusted for staff members)
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>Adjusted limits for the reader</returns>
        public ReaderLimits GetReaderLimits(Reader reader)
        {
            if (reader.IsEmployee)
            {
                // Staff members get special treatment per requirements
                return new ReaderLimits
                {
                    NMC = _config.NMC * 2,      // doubled
                    C = _config.C * 2,          // doubled  
                    D = _config.D * 2,          // doubled
                    LIM = _config.LIM * 2,      // doubled
                    DELTA = _config.DELTA / 2,  // halved
                    PER = _config.PER / 2,      // halved
                    NCZ = int.MaxValue          // ignored for staff
                };
            }
            else
            {
                // Regular readers get base configuration
                return new ReaderLimits
                {
                    NMC = _config.NMC,
                    C = _config.C,
                    D = _config.D,
                    LIM = _config.LIM,
                    DELTA = _config.DELTA,
                    PER = _config.PER,
                    NCZ = _config.NCZ
                };
            }
        }
    }
}
