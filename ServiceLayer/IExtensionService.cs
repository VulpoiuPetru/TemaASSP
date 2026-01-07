using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// The IExtensionService interface for extension management operations
    /// </summary>
    public interface IExtensionService
    {
        /// <summary>
        /// Validate and pass an extension object to the data service for creation
        /// </summary>
        /// <param name="extension">The extension</param>
        void AddExtension(Extension extension);

        /// <summary>
        /// Validate and pass an extension object to the data service for updating
        /// </summary>
        /// <param name="extension">The extension</param>
        void UpdateExtension(Extension extension);

        /// <summary>
        /// Get an extension by its identifier
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        /// <returns>The extension if found</returns>
        Extension GetExtensionById(int extensionId);

        /// <summary>
        /// Get all extensions
        /// </summary>
        /// <returns>List of all extensions</returns>
        IList<Extension> GetAllExtensions();

        /// <summary>
        /// Delete an extension by its identifier
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        void DeleteExtension(int extensionId);

        /// <summary>
        /// Get all extensions for a specific reader
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the reader</returns>
        IList<Extension> GetByReaderId(int readerId);

        /// <summary>
        /// Get all extensions for a specific book
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>List of extensions for the book</returns>
        IList<Extension> GetByBookId(int bookId);

        /// <summary>
        /// Get the total extension days for a reader in the last specified number of months
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="months">Number of months to look back</param>
        /// <returns>Total extension days</returns>
        int GetTotalExtensionDaysForReaderInLastMonths(int readerId, int months);
    }
}
