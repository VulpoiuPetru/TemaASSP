using DataMapper;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Interface for managing library configuration and thresholds
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Get the library configuration settings
        /// </summary>
        /// <returns>Library configuration</returns>
        LibraryConfiguration GetConfiguration();

        /// <summary>
        /// Get reader-specific limits (adjusted for staff members)
        /// </summary>
        /// <param name="reader">The reader</param>
        /// <returns>Adjusted limits for the reader</returns>
        ReaderLimits GetReaderLimits(Reader reader);
    }
}
