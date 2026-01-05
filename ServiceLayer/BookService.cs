using DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for book management operations with business logic validation
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IConfigurationService _configService;
        // TODO: Add repository when Data Layer is implemented
        private static List<Book> _books = new List<Book>(); // Temporary in-memory storage

        /// <summary>
        /// Initializes a new instance of the BookService class
        /// </summary>
        /// <param name="configService">Configuration service</param>
        public BookService(IConfigurationService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        /// <summary>
        /// Validate and pass a book object to the data service for creation
        /// </summary>
        /// <param name="book">The book</param>
        public void AddBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null");

            // Validate book data
            ValidateBook(book);

            // Validate domain assignment rules
            ValidateDomainAssignment(book);

            // Assign new ID (temporary - will be handled by database)
            book.BookId = _books.Count > 0 ? _books.Max(b => b.BookId) + 1 : 1;

            // Add to storage
            _books.Add(book);
        }

        /// <summary>
        /// Validate and pass a book object to the data service for updating
        /// </summary>
        /// <param name="book">The book</param>
        public void UpdateBook(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book), "Book cannot be null");

            var existingBook = GetBookById(book.BookId);
            if (existingBook == null)
                throw new InvalidOperationException($"Book with ID {book.BookId} not found");

            // Validate updated book data
            ValidateBook(book);

            // Validate domain assignment rules
            ValidateDomainAssignment(book);

            // Update the existing book
            var index = _books.FindIndex(b => b.BookId == book.BookId);
            _books[index] = book;
        }

        /// <summary>
        /// Get a book by its identifier
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <returns>The book if found</returns>
        public Book GetBookById(int bookId)
        {
            return _books.FirstOrDefault(b => b.BookId == bookId);
        }

        /// <summary>
        /// Get all books in the library
        /// </summary>
        /// <returns>List of all books</returns>
        public IList<Book> GetAllBooks()
        {
            return _books.ToList();
        }

        /// <summary>
        /// Delete a book by its identifier
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        public void DeleteBook(int bookId)
        {
            var book = GetBookById(bookId);
            if (book == null)
                throw new InvalidOperationException($"Book with ID {bookId} not found");

            // Check if book has active borrows (business rule)
            if (book.BorrowedBooks?.Any(bb => bb.BorrowEndDate > DateTime.Now) == true)
                throw new InvalidOperationException("Cannot delete book with active borrows");

            _books.RemoveAll(b => b.BookId == bookId);
        }

        /// <summary>
        /// Set domains for a book with validation
        /// </summary>
        /// <param name="bookId">The book identifier</param>
        /// <param name="domainIds">List of domain identifiers</param>
        public void SetBookDomains(int bookId, IList<int> domainIds)
        {
            var book = GetBookById(bookId);
            if (book == null)
                throw new InvalidOperationException($"Book with ID {bookId} not found");

            if (domainIds == null || !domainIds.Any())
                throw new ArgumentException("At least one domain must be specified");

            var config = _configService.GetConfiguration();

            // Validate domain count
            if (domainIds.Count > config.DOMENII)
                throw new InvalidOperationException($"Maximum allowed domains per book is {config.DOMENII}");

            // For now, just clear and add domain IDs
            // TODO: Implement proper domain validation with Data Layer
            book.Domains.Clear();
            // Will be properly implemented when Domain repository is available
        }

        /// <summary>
        /// Validates book data according to business rules
        /// </summary>
        /// <param name="book">Book to validate</param>
        private void ValidateBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new ArgumentException("Book title is required");

            if (book.Authors == null || !book.Authors.Any())
                throw new ArgumentException("Book must have at least one author");

            if (book.Domains == null || !book.Domains.Any())
                throw new ArgumentException("Book must belong to at least one domain");

            if (book.NumberOfTotalBooks < 0)
                throw new ArgumentException("Number of total books cannot be negative");

            if (book.NumberOfReadingRoomBooks < 0)
                throw new ArgumentException("Number of reading room books cannot be negative");

            if (book.NumberOfAvailableBooks < 0)
                throw new ArgumentException("Number of available books cannot be negative");

            if (book.NumberOfReadingRoomBooks > book.NumberOfTotalBooks)
                throw new ArgumentException("Reading room books cannot exceed total books");

            if (book.NumberOfAvailableBooks > book.NumberOfTotalBooks)
                throw new ArgumentException("Available books cannot exceed total books");

            if (book.Edition == null)
                throw new ArgumentException("Book must have an edition");
        }

        /// <summary>
        /// Validates domain assignment according to business rules
        /// </summary>
        /// <param name="book">Book to validate</param>
        private void ValidateDomainAssignment(Book book)
        {
            var config = _configService.GetConfiguration();

            if (book.Domains.Count > config.DOMENII)
                throw new InvalidOperationException($"Maximum allowed domains per book is {config.DOMENII}");

            // TODO: Implement ancestor-descendant validation when Domain repository is available
            // For now, just validate count
        }
    }
}
