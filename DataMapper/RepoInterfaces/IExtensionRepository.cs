using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Interface for Extension repository operations.
    /// </summary>
    public interface IExtensionRepository
    {
        /// <summary>
        /// Adds a new extension to the database.
        /// </summary>
        /// <param name="extension">The extension to add</param>
        void Add(Extension extension);

        /// <summary>
        /// Updates an existing extension in the database.
        /// </summary>
        /// <param name="extension">The extension to update</param>
        void Update(Extension extension);

        /// <summary>
        /// Deletes an extension from the database.
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        void Delete(int extensionId);

        /// <summary>
        /// Gets an extension by its identifier.
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        /// <returns>The extension if found, null otherwise</returns>
        Extension GetById(int extensionId);

        /// <summary>
        /// Gets all extensions from the database.
        /// </summary>
        /// <returns>List of all extensions</returns>
        IList<Extension> GetAll();

        /// <summary>
        /// Gets all extensions for a specific reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the reader</returns>
        IList<Extension> GetByReaderId(int readerId);

        /// <summary>
        /// Gets all extensions for a specific book.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>List of extensions for the book</returns>
        IList<Extension> GetByBookId(int bookId);

        /// <summary>
        /// Gets extensions for a specific borrowed book record.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the borrowed book</returns>
        IList<Extension> GetByBorrowedBook(int bookId, int readerId);

        /// <summary>
        /// Gets the total extension days for a reader in the last specified number of months.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="months">Number of months to look back</param>
        /// <returns>Total extension days</returns>
        int GetTotalExtensionDaysForReaderInLastMonths(int readerId, int months);

        /// <summary>
        /// Gets extensions for a reader within a specific date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of extensions within the date range</returns>
        IList<Extension> GetByReaderAndDateRange(int readerId, DateTime startDate, DateTime endDate);
    }
}
