using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for BorrowedBooks entity data access operations.
    /// </summary>
    public interface IBorrowedBooksRepository
    {
        /// <summary>
        /// Adds a new borrowed book record to the database.
        /// </summary>
        /// <param name="borrowedBook">The borrowed book to add</param>
        void Add(BorrowedBooks borrowedBook);

        /// <summary>
        /// Updates an existing borrowed book record in the database.
        /// </summary>
        /// <param name="borrowedBook">The borrowed book to update</param>
        void Update(BorrowedBooks borrowedBook);

        /// <summary>
        /// Deletes a borrowed book record from the database.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        void Delete(int bookId, int readerId);

        /// <summary>
        /// Gets a borrowed book record by book and reader identifiers.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The borrowed book if found, null otherwise</returns>
        BorrowedBooks GetByIds(int bookId, int readerId);

        /// <summary>
        /// Gets all borrowed books for a specific reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of borrowed books</returns>
        IList<BorrowedBooks> GetByReader(int readerId);

        /// <summary>
        /// Gets all currently active borrows for a reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of active borrowed books</returns>
        IList<BorrowedBooks> GetActiveByReader(int readerId);

        /// <summary>
        /// Gets borrowed books by reader in a specific date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books in the date range</returns>
        IList<BorrowedBooks> GetByReaderAndDateRange(int readerId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets borrowed books by reader for a specific book in a date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="bookId">The book identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books matching criteria</returns>
        IList<BorrowedBooks> GetByReaderBookAndDateRange(int readerId, int bookId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets borrowed books by reader for books in a specific domain in a date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="domainId">The domain identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books matching criteria</returns>
        IList<BorrowedBooks> GetByReaderDomainAndDateRange(int readerId, int domainId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all borrowed books.
        /// </summary>
        /// <returns>List of all borrowed books</returns>
        IList<BorrowedBooks> GetAll();

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
