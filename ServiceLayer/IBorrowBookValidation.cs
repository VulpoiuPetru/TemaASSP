using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;

namespace ServiceLayer
{
    /// <summary>
    /// Interface for validating book borrowing based on database constraints
    /// </summary>
    public interface IBorrowBookValidation
    {
        /// <summary>
        /// Function that validates that a book can be borrowed when 1 or more copies are not marked as being for the lecture hall
        /// </summary>
        /// <param name="book">The book to be validated</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateIfBookCanBeBorrowed(Book book);

        /// <summary>
        /// Function that validates that a book can be borrowed if the number of available books is at least 10% of the initial number of available books
        /// </summary>
        /// <param name="book">The book to be validated</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateIfThereAreAvailableCopiesToBorrow(Book book);

        /// <summary>
        /// Function that validates that a reader can borrow a maximum of C books at a time
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <param name="reader">The reader borrowing the books</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateNumberOfBorrowedBooks(IList<Book> books, Reader reader);

        /// <summary>
        /// Function that validates that if a reader borrows three or more books then all must belong to at least two different categories
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateDomainsForMoreThanThreeBooks(IList<Book> books);

        /// <summary>
        /// Function that validates that a reader can borrow a maximum of NCZ books in a single day
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <param name="reader">The reader borrowing the books</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateNumberOfBorrowedBooksPerDay(IList<Book> books, Reader reader);

        /// <summary>
        /// Function to validate that a user can borrow a maximum of NMC books in a period PER
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <param name="reader">The reader borrowing the books</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateBorrowedBooksLastPeriod(IList<Book> books, Reader reader);

        /// <summary>
        /// Function to validate that a reader cannot borrow more than D books from the same domain in the last L months
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <param name="reader">The reader borrowing the books</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateBorrowedBooksDomainsTypeLastMonths(IList<Book> books, Reader reader);

        /// <summary>
        /// Function to validate that a user can't borrow the same book in an interval DELTA
        /// </summary>
        /// <param name="books">The books to be validated</param>
        /// <param name="reader">The reader borrowing the books</param>
        /// <returns>True if the book borrowing is valid, false otherwise</returns>
        bool ValidateBorrowSameBookInPeriod(IList<Book> books, Reader reader);

        /// <summary>
        /// Function to check the sum of the extension requests of a reader in the last three months compared to the limit LIM
        /// </summary>
        /// <param name="borrowedBook">The borrowed book</param>
        /// <param name="extensionDays">Number of days to extend</param>
        /// <returns>True if the extension is valid, false otherwise</returns>
        bool ValidateExtensionRequest(BorrowedBooks borrowedBook, int extensionDays);
    }
}
