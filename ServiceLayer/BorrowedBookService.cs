using DataMapper.RepoInterfaces;
using DomainModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    /// <summary>
    /// Service for borrowing operations with comprehensive business rule validation
    /// </summary>
    public class BorrowedBookService : IBorrowedBookService
    {
        private readonly IBorrowBookValidation _borrowValidation;
        private readonly IBookService _bookService;
        private readonly IReaderService _readerService;
        private readonly IBorrowedBooksRepository _borrowedBooksRepository;
        private readonly IExtensionRepository _extensionRepository;
        private readonly ILogger<BorrowedBookService> _logger;

        /// <summary>
        /// Initializes a new instance of the BorrowedBookService class
        /// </summary>
        /// <param name="borrowValidation">Borrow validation service</param>
        /// <param name="bookService">Book service</param>
        /// <param name="readerService">Reader service</param>
        /// <param name="borrowedBooksRepository">Borrowed books repository</param>
        /// <param name="extensionRepository">Extension repository</param>
        /// <param name="logger">Logger instance</param>
        public BorrowedBookService(
            IBorrowBookValidation borrowValidation,
            IBookService bookService,
            IReaderService readerService,
            IBorrowedBooksRepository borrowedBooksRepository,
            IExtensionRepository extensionRepository,
            ILogger<BorrowedBookService> logger)
        {
            _borrowValidation = borrowValidation ?? throw new ArgumentNullException(nameof(borrowValidation));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _readerService = readerService ?? throw new ArgumentNullException(nameof(readerService));
            _borrowedBooksRepository = borrowedBooksRepository ?? throw new ArgumentNullException(nameof(borrowedBooksRepository));
            _extensionRepository = extensionRepository ?? throw new ArgumentNullException(nameof(extensionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Validate the books to be borrowed and the reader borrowing them and pass borrowed books to the data service for creation
        /// </summary>
        /// <param name="books">The books list</param>
        /// <param name="reader">The reader borrowing the books</param>
        public void BorrowBooks(IList<Book> books, Reader reader)
        {
            try
            {
                if (books == null || !books.Any())
                    throw new ArgumentException("At least one book must be specified for borrowing");

                if (reader == null)
                    throw new ArgumentNullException(nameof(reader), "Reader cannot be null");

                var existingReader = _readerService.GetReaderById(reader.ReaderId);
                if (existingReader == null)
                    throw new InvalidOperationException($"Reader with ID {reader.ReaderId} not found");

                // Validate each book individually
                foreach (var book in books)
                {
                    var existingBook = _bookService.GetBookById(book.BookId);
                    if (existingBook == null)
                        throw new InvalidOperationException($"Book with ID {book.BookId} not found");

                    if (!_borrowValidation.ValidateIfBookCanBeBorrowed(existingBook))
                        throw new InvalidOperationException($"Book '{existingBook.Title}' cannot be borrowed - all copies are for reading room only");

                    if (!_borrowValidation.ValidateIfThereAreAvailableCopiesToBorrow(existingBook))
                        throw new InvalidOperationException($"Book '{existingBook.Title}' cannot be borrowed - insufficient copies available (10% rule)");
                }

                // Validate borrowing rules
                if (!_borrowValidation.ValidateNumberOfBorrowedBooks(books, reader))
                    throw new InvalidOperationException("Exceeds maximum number of books allowed per borrowing session");

                if (!_borrowValidation.ValidateDomainsForMoreThanThreeBooks(books))
                    throw new InvalidOperationException("When borrowing 3 or more books, they must belong to at least 2 different domains");

                if (!_borrowValidation.ValidateNumberOfBorrowedBooksPerDay(books, reader))
                    throw new InvalidOperationException("Exceeds maximum number of books allowed per day");

                if (!_borrowValidation.ValidateBorrowedBooksLastPeriod(books, reader))
                    throw new InvalidOperationException("Exceeds maximum number of books allowed in the specified period");

                if (!_borrowValidation.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader))
                    throw new InvalidOperationException("Exceeds maximum number of books from the same domain in recent months");

                if (!_borrowValidation.ValidateBorrowSameBookInPeriod(books, reader))
                    throw new InvalidOperationException("Cannot borrow the same book within the specified interval");

                // Create borrowed book records
                var borrowDate = DateTime.Now;
                var returnDate = borrowDate.AddDays(14); // Default 14-day loan period

                foreach (var book in books)
                {
                    var borrowedBook = new BorrowedBooks
                    {
                        BookId = book.BookId,
                        ReaderId = reader.ReaderId,
                        BorrowStartDate = borrowDate,
                        BorrowEndDate = returnDate,
                        Book = book,
                        Reader = reader
                    };

                    _borrowedBooksRepository.Add(borrowedBook);

                    // Update book availability
                    var existingBook = _bookService.GetBookById(book.BookId);
                    if (existingBook != null && existingBook.NumberOfAvailableBooks > 0)
                    {
                        existingBook.NumberOfAvailableBooks--;
                        _bookService.UpdateBook(existingBook);
                    }
                }

                _borrowedBooksRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error borrowing books for reader ID: {ReaderId}", reader?.ReaderId);
                throw;
            }
        }

        /// <summary>
        /// Validate and pass a borrowed book to the data service for updating (extension)
        /// </summary>
        /// <param name="borrowedBook">The borrowed book</param>
        /// <param name="extensionDays">Number of days to extend</param>
        public void ExtendBorrowedBook(BorrowedBooks borrowedBook, int extensionDays)
        {
            try
            {
                if (borrowedBook == null)
                    throw new ArgumentNullException(nameof(borrowedBook), "Borrowed book cannot be null");

                if (extensionDays <= 0 || extensionDays > 90)
                    throw new ArgumentException("Extension days must be between 1 and 90");

                var existingBorrow = _borrowedBooksRepository.GetByIds(borrowedBook.BookId, borrowedBook.ReaderId);
                if (existingBorrow == null)
                    throw new InvalidOperationException("Borrowed book record not found");

                if (existingBorrow.BorrowEndDate <= DateTime.Now)
                    throw new InvalidOperationException("Cannot extend an overdue book");

                if (!_borrowValidation.ValidateExtensionRequest(existingBorrow, extensionDays))
                    throw new InvalidOperationException("Extension request exceeds allowed limits");

                // Apply extension
                if (existingBorrow.BorrowEndDateExtended == DateTime.MinValue)
                {
                    existingBorrow.BorrowEndDateExtended = existingBorrow.BorrowEndDate.AddDays(extensionDays);
                }
                else
                {
                    existingBorrow.BorrowEndDateExtended = existingBorrow.BorrowEndDateExtended.AddDays(extensionDays);
                }

                _borrowedBooksRepository.Update(existingBorrow);

                // Create extension record
                var extension = new Extension
                {
                    BookId = borrowedBook.BookId,
                    ReaderId = borrowedBook.ReaderId,
                    RequestDate = DateTime.Now,
                    ExtensionDays = extensionDays,
                    BorrowedBooks = existingBorrow
                };

                _extensionRepository.Add(extension);

                // Update reader's extension count
                var reader = _readerService.GetReaderById(borrowedBook.ReaderId);
                if (reader != null)
                {
                    reader.NumberOfExtensions++;
                    _readerService.UpdateReader(reader);
                }

                _borrowedBooksRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extending borrowed book - BookId: {BookId}, ReaderId: {ReaderId}",
                    borrowedBook?.BookId, borrowedBook?.ReaderId);
                throw;
            }
        }

        /// <summary>
        /// Return a borrowed book to the library
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="bookId">The book identifier</param>
        public void ReturnBook(int readerId, int bookId)
        {
            try
            {
                var borrowedBook = _borrowedBooksRepository.GetByIds(bookId, readerId);
                if (borrowedBook == null)
                    throw new InvalidOperationException("Borrowed book record not found");

                _borrowedBooksRepository.Delete(bookId, readerId);

                var book = _bookService.GetBookById(bookId);
                if (book != null)
                {
                    book.NumberOfAvailableBooks++;
                    _bookService.UpdateBook(book);
                }

                _borrowedBooksRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning book - BookId: {BookId}, ReaderId: {ReaderId}", bookId, readerId);
                throw;
            }
        }

        /// <summary>
        /// Get all borrowed books for a specific reader
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of borrowed books</returns>
        public IList<BorrowedBooks> GetBorrowedBooksByReader(int readerId)
        {
            try
            {
                return _borrowedBooksRepository.GetByReader(readerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving borrowed books for reader ID: {ReaderId}", readerId);
                throw;
            }
        }

        /// <summary>
        /// Get all currently borrowed books in the library
        /// </summary>
        /// <returns>List of all borrowed books</returns>
        public IList<BorrowedBooks> GetAllBorrowedBooks()
        {
            try
            {
                return _borrowedBooksRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all borrowed books");
                throw;
            }
        }
    }
}
