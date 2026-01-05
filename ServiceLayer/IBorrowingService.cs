using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMapper;


namespace ServiceLayer
{
    /// <summary>
    /// The IBorrowedBookService interface for borrowing operations
    /// </summary>
    public interface IBorrowedBookService
    {
        /// <summary>
        /// Validate the books to be borrowed and the reader borrowing them and pass borrowed books to the data service for creation
        /// </summary>
        /// <param name="books">The books list</param>
        /// <param name="reader">The reader borrowing the books</param>
        void BorrowBooks(IList<Book> books, Reader reader);

        /// <summary>
        /// Validate and pass a borrowed book to the data service for updating (extension)
        /// </summary>
        /// <param name="borrowedBook">The borrowed book</param>
        /// <param name="extensionDays">Number of days to extend</param>
        void ExtendBorrowedBook(BorrowedBooks borrowedBook, int extensionDays);

        /// <summary>
        /// Return a borrowed book to the library
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="bookId">The book identifier</param>
        void ReturnBook(int readerId, int bookId);

        /// <summary>
        /// Get all borrowed books for a specific reader
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of borrowed books</returns>
        IList<BorrowedBooks> GetBorrowedBooksByReader(int readerId);

        /// <summary>
        /// Get all currently borrowed books in the library
        /// </summary>
        /// <returns>List of all borrowed books</returns>
        IList<BorrowedBooks> GetAllBorrowedBooks();
    }
}
