using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for extension data access operations
    /// </summary>
    public interface IExtensionDataServices
    {
        /// <summary>
        /// Adds a new extension record to the database
        /// </summary>
        /// <param name="extension">The extension</param>
        void AddExtension(Extension extension);

        /// <summary>
        /// Retrieves an extension from the database by id
        /// </summary>
        /// <param name="extensionId">The extension's id</param>
        /// <returns>An Extension object containing the info from the database</returns>
        Extension GetExtensionById(int extensionId);

        /// <summary>
        /// Retrieves all extensions for a specific reader from the database
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <returns>A list containing all extensions for the reader</returns>
        IList<Extension> GetExtensionsByReader(int readerId);

        /// <summary>
        /// Retrieves extensions for a reader in a specific date period
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A list containing extensions in the specified period</returns>
        IList<Extension> GetExtensionsByReaderInPeriod(int readerId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves all extensions for a specific borrowed book
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="bookId">The book's id</param>
        /// <returns>A list containing all extensions for the borrowed book</returns>
        IList<Extension> GetExtensionsByBorrowedBook(int readerId, int bookId);

        /// <summary>
        /// Retrieves all extensions from the database
        /// </summary>
        /// <returns>A list containing all extensions from the database</returns>
        IList<Extension> GetAllExtensions();

        /// <summary>
        /// Retrieves extensions by request date from the database
        /// </summary>
        /// <param name="requestDate">The request date</param>
        /// <returns>A list containing extensions requested on the specified date</returns>
        IList<Extension> GetExtensionsByRequestDate(DateTime requestDate);

        /// <summary>
        /// Updates an existing extension record in the database
        /// </summary>
        /// <param name="extension">The updated extension</param>
        void UpdateExtension(Extension extension);

        /// <summary>
        /// Deletes an existing extension from the database by id
        /// </summary>
        /// <param name="extensionId">The extension's id</param>
        void DeleteExtensionById(int extensionId);
    }
}
