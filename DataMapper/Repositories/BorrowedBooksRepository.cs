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
    /// Repository implementation for BorrowedBooks entity data access operations.
    /// </summary>
    public class BorrowedBooksRepository : IBorrowedBooksRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BorrowedBooksRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public BorrowedBooksRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new borrowed book record to the database.
        /// </summary>
        /// <param name="borrowedBook">The borrowed book to add</param>
        public void Add(BorrowedBooks borrowedBook)
        {
            if (borrowedBook == null)
                throw new ArgumentNullException(nameof(borrowedBook));

            _context.BorrowedBooks.Add(borrowedBook);
        }

        /// <summary>
        /// Updates an existing borrowed book record in the database.
        /// </summary>
        /// <param name="borrowedBook">The borrowed book to update</param>
        public void Update(BorrowedBooks borrowedBook)
        {
            if (borrowedBook == null)
                throw new ArgumentNullException(nameof(borrowedBook));

            _context.Entry(borrowedBook).State = EntityState.Modified;
        }

        /// <summary>
        /// Deletes a borrowed book record from the database.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        public void Delete(int bookId, int readerId)
        {
            var borrowedBook = GetByIds(bookId, readerId);
            if (borrowedBook != null)
            {
                _context.BorrowedBooks.Remove(borrowedBook);
            }
        }

        /// <summary>
        /// Gets a borrowed book record by book and reader identifiers.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>The borrowed book if found, null otherwise</returns>
        public BorrowedBooks GetByIds(int bookId, int readerId)
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Book.Domains)
                .Include(bb => bb.Reader)
                .FirstOrDefault(bb => bb.BookId == bookId && bb.ReaderId == readerId);
        }

        /// <summary>
        /// Gets all borrowed books for a specific reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of borrowed books</returns>
        public IList<BorrowedBooks> GetByReader(int readerId)
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Book.Domains)
                .Include(bb => bb.Reader)
                .Where(bb => bb.ReaderId == readerId)
                .ToList();
        }

        /// <summary>
        /// Gets all currently active borrows for a reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of active borrowed books</returns>
        public IList<BorrowedBooks> GetActiveByReader(int readerId)
        {
            var currentDate = DateTime.Now;
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Reader)
                .Where(bb => bb.ReaderId == readerId)
                .Where(bb => bb.BorrowEndDate > currentDate ||
                            (bb.BorrowEndDateExtended != DateTime.MinValue && bb.BorrowEndDateExtended > currentDate))
                .ToList();
        }

        /// <summary>
        /// Gets borrowed books by reader in a specific date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books in the date range</returns>
        public IList<BorrowedBooks> GetByReaderAndDateRange(int readerId, DateTime startDate, DateTime endDate)
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Book.Domains)
                .Include(bb => bb.Reader)
                .Where(bb => bb.ReaderId == readerId)
                .Where(bb => bb.BorrowStartDate >= startDate && bb.BorrowStartDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Gets borrowed books by reader for a specific book in a date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="bookId">The book identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books matching criteria</returns>
        public IList<BorrowedBooks> GetByReaderBookAndDateRange(int readerId, int bookId, DateTime startDate, DateTime endDate)
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Reader)
                .Where(bb => bb.ReaderId == readerId && bb.BookId == bookId)
                .Where(bb => bb.BorrowStartDate >= startDate && bb.BorrowStartDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Gets borrowed books by reader for books in a specific domain in a date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="domainId">The domain identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of borrowed books matching criteria</returns>
        public IList<BorrowedBooks> GetByReaderDomainAndDateRange(int readerId, int domainId, DateTime startDate, DateTime endDate)
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Book.Domains)
                .Include(bb => bb.Reader)
                .Where(bb => bb.ReaderId == readerId)
                .Where(bb => bb.Book.Domains.Any(d => d.DomainId == domainId))
                .Where(bb => bb.BorrowStartDate >= startDate && bb.BorrowStartDate <= endDate)
                .ToList();
        }

        /// <summary>
        /// Gets all borrowed books.
        /// </summary>
        /// <returns>List of all borrowed books</returns>
        public IList<BorrowedBooks> GetAll()
        {
            return _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.Reader)
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
