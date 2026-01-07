using DataMapper.RepoInterfaces;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataMapper.Repositories
{
    /// <summary>
    /// Repository implementation for Extension entity using Data Mapper pattern.
    /// </summary>
    public class ExtensionRepository : IExtensionRepository
    {
        private readonly LibraryContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context</param>
        public ExtensionRepository(LibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new extension to the database.
        /// </summary>
        /// <param name="extension">The extension to add</param>
        public void Add(Extension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            _context.Extensions.Add(extension);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates an existing extension in the database.
        /// </summary>
        /// <param name="extension">The extension to update</param>
        public void Update(Extension extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            var existingExtension = _context.Extensions.Find(extension.ExtensionId);
            if (existingExtension == null)
                throw new InvalidOperationException($"Extension with ID {extension.ExtensionId} not found");

            _context.Entry(existingExtension).CurrentValues.SetValues(extension);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes an extension from the database.
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        public void Delete(int extensionId)
        {
            var extension = _context.Extensions.Find(extensionId);
            if (extension == null)
                throw new InvalidOperationException($"Extension with ID {extensionId} not found");

            _context.Extensions.Remove(extension);
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets an extension by its identifier.
        /// </summary>
        /// <param name="extensionId">The extension identifier</param>
        /// <returns>The extension if found, null otherwise</returns>
        public Extension GetById(int extensionId)
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .FirstOrDefault(e => e.ExtensionId == extensionId);
        }

        /// <summary>
        /// Gets all extensions from the database.
        /// </summary>
        /// <returns>List of all extensions</returns>
        public IList<Extension> GetAll()
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .ToList();
        }

        /// <summary>
        /// Gets all extensions for a specific reader.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the reader</returns>
        public IList<Extension> GetByReaderId(int readerId)
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .Where(e => e.ReaderId == readerId)
                .ToList();
        }

        /// <summary>
        /// Gets all extensions for a specific book.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>List of extensions for the book</returns>
        public IList<Extension> GetByBookId(int bookId)
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .Where(e => e.BookId == bookId)
                .ToList();
        }

        /// <summary>
        /// Gets extensions for a specific borrowed book record.
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of extensions for the borrowed book</returns>
        public IList<Extension> GetByBorrowedBook(int bookId, int readerId)
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .Where(e => e.BookId == bookId && e.ReaderId == readerId)
                .ToList();
        }

        /// <summary>
        /// Gets the total extension days for a reader in the last specified number of months.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="months">Number of months to look back</param>
        /// <returns>Total extension days</returns>
        public int GetTotalExtensionDaysForReaderInLastMonths(int readerId, int months)
        {
            var startDate = DateTime.Now.AddMonths(-months);

            var totalDays = _context.Extensions
                .Where(e => e.ReaderId == readerId && e.RequestDate >= startDate)
                .Sum(e => (int?)e.ExtensionDays) ?? 0;

            return totalDays;
        }

        /// <summary>
        /// Gets extensions for a reader within a specific date range.
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>List of extensions within the date range</returns>
        public IList<Extension> GetByReaderAndDateRange(int readerId, DateTime startDate, DateTime endDate)
        {
            return _context.Extensions
                .Include(e => e.BorrowedBooks)
                .Include(e => e.BorrowedBooks.Book)
                .Include(e => e.BorrowedBooks.Reader)
                .Where(e => e.ReaderId == readerId
                    && e.RequestDate >= startDate
                    && e.RequestDate <= endDate)
                .ToList();
        }
    }
}
