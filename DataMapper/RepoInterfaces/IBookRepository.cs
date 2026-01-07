using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.RepoInterfaces
{
    /// <summary>
    /// Repository interface for Book entity data access operations.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="book">The book to add</param>
        void Add(Book book);

        /// <summary>
        /// Updates an existing book in the database.
        /// </summary>
        /// <param name="book">The book to update</param>
        void Update(Book book);

        /// <summary>
        /// Deletes a book from the database by its identifier.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        void Delete(int bookId);

        /// <summary>
        /// Gets a book by its identifier.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The book if found, null otherwise</returns>
        Book GetById(int bookId);

        /// <summary>
        /// Gets all books from the database.
        /// </summary>
        /// <returns>List of all books</returns>
        IList<Book> GetAll();

        /// <summary>
        /// Gets books by domain identifier (including parent domains).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of books in the specified domain</returns>
        IList<Book> GetByDomain(int domainId);

        /// <summary>
        /// Gets books by author identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>List of books by the specified author</returns>
        IList<Book> GetByAuthor(int authorId);

        /// <summary>
        /// Gets available books for borrowing.
        /// </summary>
        /// <returns>List of books that can be borrowed</returns>
        IList<Book> GetAvailableForBorrowing();

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        void SaveChanges();
    }
}
