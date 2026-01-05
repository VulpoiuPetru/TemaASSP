using DataMapper;
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

        // TODO: Add repository when Data Layer is implemented
        private static List<BorrowedBooks> _borrowedBooks = new List<BorrowedBooks>(); // Temporary in-memory storage

        /// <summary>
        /// Initializes a new instance of the BorrowedBookService class
        /// </summary>
        public BorrowedBookService(
            IBorrowBookValidation borrowValidation,
            IBookService bookService,
            IReaderService readerService)
        {
            _borrowValidation = borrowValidation ?? throw new ArgumentNullException(nameof(borrowValidation));
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _readerService = readerService ?? throw new ArgumentNullException(nameof(readerService));
        }

        /// <summary>
        /// Validate the books to be borrowed and the reader borrowing them and pass borrowed books to the data service for creation
        /// </summary>
        /// <param name="books">The books list</param>
        /// <param name="reader">The reader borrowing the books</param>
        public void BorrowBooks(IList<Book> books, Reader reader)
        {
            if (books == null || !books.Any())
                throw new ArgumentException("At least one book must be specified for borrowing");

            if (reader == null)
                throw new ArgumentNullException(nameof(reader), "Reader cannot be null");

            // Verify reader exists
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

                _borrowedBooks.Add(borrowedBook);

                // Update book availability
                var existingBook = _bookService.GetBookById(book.BookId);
                if (existingBook != null && existingBook.NumberOfAvailableBooks > 0)
                {
                    existingBook.NumberOfAvailableBooks--;
                    _bookService.UpdateBook(existingBook);
                }
            }
        }

        /// <summary>
        /// Validate and pass a borrowed book to the data service for updating (extension)
        /// </summary>
        /// <param name="borrowedBook">The borrowed book</param>
        /// <param name="extensionDays">Number of days to extend</param>
        public void ExtendBorrowedBook(BorrowedBooks borrowedBook, int extensionDays)
        {
            if (borrowedBook == null)
                throw new ArgumentNullException(nameof(borrowedBook), "Borrowed book cannot be null");

            if (extensionDays <= 0 || extensionDays > 90)
                throw new ArgumentException("Extension days must be between 1 and 90");

            var existingBorrow = _borrowedBooks.FirstOrDefault(bb =>
                bb.BookId == borrowedBook.BookId && bb.ReaderId == borrowedBook.ReaderId);

            if (existingBorrow == null)
                throw new InvalidOperationException("Borrowed book record not found");

            if (existingBorrow.BorrowEndDate <= DateTime.Now)
                throw new InvalidOperationException("Cannot extend an overdue book");

            // Validate extension request
            if (!_borrowValidation.ValidateExtensionRequest(borrowedBook, extensionDays))
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

            // Update reader's extension count
            var reader = _readerService.GetReaderById(borrowedBook.ReaderId);
            if (reader != null)
            {
                reader.NumberOfExtensions++;
                _readerService.UpdateReader(reader);
            }
        }

        /// <summary>
        /// Return a borrowed book to the library
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <param name="bookId">The book identifier</param>
        public void ReturnBook(int readerId, int bookId)
        {
            var borrowedBook = _borrowedBooks.FirstOrDefault(bb =>
                bb.BookId == bookId && bb.ReaderId == readerId);

            if (borrowedBook == null)
                throw new InvalidOperationException("Borrowed book record not found");

            // Remove from borrowed books
            _borrowedBooks.Remove(borrowedBook);

            // Update book availability
            var book = _bookService.GetBookById(bookId);
            if (book != null)
            {
                book.NumberOfAvailableBooks++;
                _bookService.UpdateBook(book);
            }
        }

        /// <summary>
        /// Get all borrowed books for a specific reader
        /// </summary>
        /// <param name="readerId">The reader identifier</param>
        /// <returns>List of borrowed books</returns>
        public IList<BorrowedBooks> GetBorrowedBooksByReader(int readerId)
        {
            return _borrowedBooks.Where(bb => bb.ReaderId == readerId).ToList();
        }

        /// <summary>
        /// Get all currently borrowed books in the library
        /// </summary>
        /// <returns>List of all borrowed books</returns>
        public IList<BorrowedBooks> GetAllBorrowedBooks()
        {
            return _borrowedBooks.ToList();
        }
    }
}
