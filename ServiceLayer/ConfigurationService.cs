using Microsoft.Extensions.Configuration;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// Initializes a new instance of the ConfigurationService class with default values
        /// </summary>
        public ConfigurationService()
        {
            // Default configuration values (can be loaded from file later)
            _config = new LibraryConfiguration
            {
                DOMENII = 5,
                NMC = 10,
                PER = 30,
                C = 5,
                D = 3,
                L = 6,
                LIM = 15,
                DELTA = 14,
                NCZ = 3,
                PERSIMP = 20
            };
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
