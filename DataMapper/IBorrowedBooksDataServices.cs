using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper
{
    /// <summary>
    /// Interface for borrowed books data access operations
    /// </summary>
    public interface IBorrowedBooksDataServices
    {
        /// <summary>
        /// Adds a new borrowed book record to the database
        /// </summary>
        /// <param name="borrowedBook">The borrowed book</param>
        void AddBorrowedBook(BorrowedBooks borrowedBook);

        /// <summary>
        /// Retrieves a borrowed book from the database by reader and book ids
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="bookId">The book's id</param>
        /// <returns>A BorrowedBooks object containing the info from the database</returns>
        BorrowedBooks GetBorrowedBookByReaderAndBook(int readerId, int bookId);

        /// <summary>
        /// Retrieves all borrowed books for a specific reader
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <returns>A list containing all borrowed books for the reader</returns>
        IList<BorrowedBooks> GetBorrowedBooksByReader(int readerId);

        /// <summary>
        /// Retrieves all borrowed books for a specific book
        /// </summary>
        /// <param name="bookId">The book's id</param>
        /// <returns>A list containing all borrowed book records for the book</returns>
        IList<BorrowedBooks> GetBorrowedBooksByBook(int bookId);

        /// <summary>
        /// Retrieves all currently active borrowed books
        /// </summary>
        /// <returns>A list containing all active borrowed books</returns>
        IList<BorrowedBooks> GetAllActiveBorrowedBooks();

        /// <summary>
        /// Retrieves borrowed books by reader on a specific date
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="date">The specific date</param>
        /// <returns>A list containing borrowed books on that date</returns>
        IList<BorrowedBooks> GetBorrowedBooksByReaderAndDate(int readerId, DateTime date);

        /// <summary>
        /// Retrieves borrowed books by reader in a date period
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>A list containing borrowed books in the period</returns>
        IList<BorrowedBooks> GetBorrowedBooksByReaderInPeriod(int readerId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Retrieves all borrowed books from the database
        /// </summary>
        /// <returns>A list containing all borrowed books</returns>
        IList<BorrowedBooks> GetAllBorrowedBooks();

        /// <summary>
        /// Updates an existing borrowed book record in the database
        /// </summary>
        /// <param name="borrowedBook">The updated borrowed book</param>
        void UpdateBorrowedBook(BorrowedBooks borrowedBook);

        /// <summary>
        /// Deletes a borrowed book record from the database
        /// </summary>
        /// <param name="readerId">The reader's id</param>
        /// <param name="bookId">The book's id</param>
        void DeleteBorrowedBookByIds(int readerId, int bookId);
    }
}
