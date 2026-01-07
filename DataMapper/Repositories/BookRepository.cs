using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMapper.Repositories
{
    /// <summary>
    /// Repository implementation for Book entity data access operations.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public BookRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new book to the database.
        /// </summary>
        /// <param name="book">The book to add</param>
        public void Add(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _context.Books.Add(book);
        }

        /// <summary>
        /// Updates an existing book in the database.
        /// </summary>
        /// <param name="book">The book to update</param>
        public void Update(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            _context.Entry(book).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a book from the database by its identifier.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        public void Delete(int bookId)
        {
            var book = GetById(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
        }

        /// <summary>
        /// Gets a book by its identifier.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The book if found, null otherwise</returns>
        public Book GetById(int bookId)
        {
            return _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Domains)
                .Include(b => b.Edition)
                .Include(b => b.Edition.Copies)
                .Include(b => b.BorrowedBooks)
                .FirstOrDefault(b => b.BookId == bookId);
        }

        /// <summary>
        /// Gets all books from the database.
        /// </summary>
        /// <returns>List of all books</returns>
        public IList<Book> GetAll()
        {
            return _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Domains)
                .Include(b => b.Edition)
                .ToList();
        }

        /// <summary>
        /// Gets books by domain identifier (including parent domains).
        /// </summary>
        /// <param name="domainId">The domain identifier</param>
        /// <returns>List of books in the specified domain</returns>
        public IList<Book> GetByDomain(int domainId)
        {
            return _context.Books
                .Include(b => b.Domains)
                .Where(b => b.Domains.Any(d => d.DomainId == domainId))
                .ToList();
        }

        /// <summary>
        /// Gets books by author identifier.
        /// </summary>
        /// <param name="authorId">The author identifier</param>
        /// <returns>List of books by the specified author</returns>
        public IList<Book> GetByAuthor(int authorId)
        {
            return _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Authors.Any(a => a.AuthorId == authorId))
                .ToList();
        }

        /// <summary>
        /// Gets available books for borrowing.
        /// </summary>
        /// <returns>List of books that can be borrowed</returns>
        public IList<Book> GetAvailableForBorrowing()
        {
            return _context.Books
                .Include(b => b.Edition)
                .Include(b => b.Edition.Copies)
                .Where(b => b.NumberOfAvailableBooks > 0)
                .Where(b => b.NumberOfTotalBooks > b.NumberOfReadingRoomBooks)
                .ToList();
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
