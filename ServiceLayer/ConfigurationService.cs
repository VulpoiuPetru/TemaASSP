using DataMapper;
using Microsoft.Extensions.Configuration;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for managing library configuration and calculating reader-specific limits
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private LibraryConfiguration _libraryConfig;

        /// <summary>
        /// Initializes a new instance of the ConfigurationService class
        /// </summary>
        /// <param name="configuration">Configuration provider</param>
        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Get the library configuration settings (loads from external config)
        /// </summary>
        /// <returns>Library configuration</returns>
        public LibraryConfiguration GetConfiguration()
        {
            if (_libraryConfig == null)
            {
                _libraryConfig = _configuration.GetSection("LibraryConfiguration").Get<LibraryConfiguration>()
                    ?? throw new InvalidOperationException("LibraryConfiguration section not found in configuration");
            }
            return _libraryConfig;
        }

        /// <summary>
        /// Get reader-specific limits (adjusted for staff members)
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>Adjusted limits for the reader</returns>
        public ReaderLimits GetReaderLimits(Reader reader)
        {
            var config = GetConfiguration();

            if (reader.IsEmployee)
            {
                // Staff members get special treatment per requirements
                return new ReaderLimits
                {
                    NMC = config.NMC * 2,      // doubled
                    C = config.C * 2,          // doubled  
                    D = config.D * 2,          // doubled
                    LIM = config.LIM * 2,      // doubled
                    DELTA = config.DELTA / 2,  // halved
                    PER = config.PER / 2,      // halved
                    NCZ = int.MaxValue         // ignored for staff
                };
            }
            else
            {
                // Regular readers get base configuration
                return new ReaderLimits
                {
                    NMC = config.NMC,
                    C = config.C,
                    D = config.D,
                    LIM = config.LIM,
                    DELTA = config.DELTA,
                    PER = config.PER,
                    NCZ = config.NCZ
                };
            }
        }
    }
}
