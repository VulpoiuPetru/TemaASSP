using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel;

namespace ServiceLayer
{
    /// <summary>
    /// The IBookService interface for book management operations
    /// </summary>
    public interface IBookService
    {
        /// <summary>
        /// Validate and pass a book object to the data service for creation
        /// </summary>
        /// <param name="book">The book</param>
        void AddBook(Book book);

        /// <summary>
        /// Validate and pass a book object to the data service for updating
        /// </summary>
        /// <param name="book">The book</param>
        void UpdateBook(Book book);

        /// <summary>
        /// Get a book by its identifier
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The book if found</returns>
        Book GetBookById(int bookId);

        /// <summary>
        /// Get all books in the library
        /// </summary>
        /// <returns>List of all books</returns>
        IList<Book> GetAllBooks();

        /// <summary>
        /// Delete a book by its identifier
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        void DeleteBook(int bookId);

        /// <summary>
        /// Set domains for a book with validation
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="domainIds">List of domain identifiers</param>
        void SetBookDomains(int bookId, IList<int> domainIds);
    }
}
