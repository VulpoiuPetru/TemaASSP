using DataMapper.RepoInterfaces;
using DomainModel;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceLayer;
using System;
using System.Collections.Generic;

namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the BorrowedBookService class
    /// </summary>
    [TestClass]
    public class BorrowedBookServiceTests
    {
        private Mock<IBorrowBookValidation> _mockBorrowValidation;
        private Mock<IBookService> _mockBookService;
        private Mock<IReaderService> _mockReaderService;
        private Mock<IBorrowedBooksRepository> _mockBorrowedBooksRepository;
        private Mock<IExtensionRepository> _mockExtensionRepository;
        private Mock<IConfigurationService> _mockConfigService;
        private Mock<ILogger<BorrowedBookService>> _mockLogger;
        private BorrowedBookService _borrowedBookService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockBorrowValidation = new Mock<IBorrowBookValidation>();
            _mockBookService = new Mock<IBookService>();
            _mockReaderService = new Mock<IReaderService>();
            _mockBorrowedBooksRepository = new Mock<IBorrowedBooksRepository>();
            _mockExtensionRepository = new Mock<IExtensionRepository>();
            _mockConfigService = new Mock<IConfigurationService>();
            _mockLogger = new Mock<ILogger<BorrowedBookService>>();

            _borrowedBookService = new BorrowedBookService(
                _mockBorrowValidation.Object,
                _mockBookService.Object,
                _mockReaderService.Object,
                _mockBorrowedBooksRepository.Object,
                _mockExtensionRepository.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBorrowBooks_WithEmptyBookList_ThrowsException()
        {
            var reader = new Reader { ReaderId = 1 };
            _borrowedBookService.BorrowBooks(new List<Book>(), reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestBorrowBooks_WithNullReader_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            _borrowedBookService.BorrowBooks(books, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_WithNonExistentReader_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 999 };

            _mockReaderService.Setup(s => s.GetReaderById(999)).Returns((Reader)null);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_WithNonExistentBook_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 999 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(999)).Returns((Book)null);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_WithReadingRoomOnlyBook_ThrowsException()
        {
            var book = new Book { BookId = 1, Title = "Test Book" };
            var books = new List<Book> { book };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(book);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(book)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_WithInsufficientCopies_ThrowsException()
        {
            var book = new Book { BookId = 1, Title = "Test Book" };
            var books = new List<Book> { book };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(book);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(book)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(book)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_ExceedsMaxBooksPerSession_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(books[0]);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_ThreeBooksOneDomain_ThrowsException()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1 },
                new Book { BookId = 2 },
                new Book { BookId = 3 }
            };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            foreach (var book in books)
            {
                _mockBookService.Setup(s => s.GetBookById(book.BookId)).Returns(book);
            }
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_ExceedsDailyLimit_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(books[0]);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooksPerDay(books, reader)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_ExceedsPeriodLimit_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(books[0]);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooksPerDay(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksLastPeriod(books, reader)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_ExceedsDomainLimit_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(books[0]);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooksPerDay(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksLastPeriod(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBorrowBooks_SameBookRecentlyBorrowed_ThrowsException()
        {
            var books = new List<Book> { new Book { BookId = 1 } };
            var reader = new Reader { ReaderId = 1 };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(books[0]);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooksPerDay(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksLastPeriod(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowSameBookInPeriod(books, reader)).Returns(false);

            _borrowedBookService.BorrowBooks(books, reader);
        }

        [TestMethod]
        public void TestBorrowBooks_ValidRequest_CreatesRecordsAndUpdatesAvailability()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                NumberOfAvailableBooks = 10
            };
            var books = new List<Book> { book };
            var reader = new Reader { ReaderId = 1, IsEmployee = false };

            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(book);
            _mockBorrowValidation.Setup(v => v.ValidateIfBookCanBeBorrowed(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateIfThereAreAvailableCopiesToBorrow(It.IsAny<Book>())).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooks(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateDomainsForMoreThanThreeBooks(books)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateNumberOfBorrowedBooksPerDay(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksLastPeriod(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowedBooksDomainsTypeLastMonths(books, reader)).Returns(true);
            _mockBorrowValidation.Setup(v => v.ValidateBorrowSameBookInPeriod(books, reader)).Returns(true);

            _borrowedBookService.BorrowBooks(books, reader);

            _mockBorrowedBooksRepository.Verify(r => r.Add(It.IsAny<BorrowedBooks>()), Times.Once);
            _mockBorrowedBooksRepository.Verify(r => r.SaveChanges(), Times.Once);
            _mockBookService.Verify(s => s.UpdateBook(It.Is<Book>(b => b.NumberOfAvailableBooks == 9)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestExtendBorrowedBook_WithNullBorrowedBook_ThrowsException()
        {
            _borrowedBookService.ExtendBorrowedBook(null, 7);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtendBorrowedBook_WithZeroDays_ThrowsException()
        {
            var borrowedBook = new BorrowedBooks { BookId = 1, ReaderId = 1 };
            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestExtendBorrowedBook_WithTooManyDays_ThrowsException()
        {
            var borrowedBook = new BorrowedBooks { BookId = 1, ReaderId = 1 };
            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestExtendBorrowedBook_WithNonExistentRecord_ThrowsException()
        {
            var borrowedBook = new BorrowedBooks { BookId = 999, ReaderId = 999 };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(999, 999)).Returns((BorrowedBooks)null);

            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 7);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestExtendBorrowedBook_WithOverdueBook_ThrowsException()
        {
            var borrowedBook = new BorrowedBooks
            {
                BookId = 1,
                ReaderId = 1,
                BorrowEndDate = DateTime.Now.AddDays(-5)
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(1, 1)).Returns(borrowedBook);

            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 7);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestExtendBorrowedBook_ExceedsExtensionLimit_ThrowsException()
        {
            var borrowedBook = new BorrowedBooks
            {
                BookId = 1,
                ReaderId = 1,
                BorrowEndDate = DateTime.Now.AddDays(5)
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(1, 1)).Returns(borrowedBook);
            _mockBorrowValidation.Setup(v => v.ValidateExtensionRequest(borrowedBook, 7)).Returns(false);

            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 7);
        }

        [TestMethod]
        public void TestExtendBorrowedBook_ValidRequest_ExtendsAndCreatesRecord()
        {
            var reader = new Reader { ReaderId = 1, NumberOfExtensions = 0 };
            var borrowedBook = new BorrowedBooks
            {
                BookId = 1,
                ReaderId = 1,
                BorrowEndDate = DateTime.Now.AddDays(5),
                BorrowEndDateExtended = DateTime.MinValue,
                Reader = reader
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(1, 1)).Returns(borrowedBook);
            _mockBorrowValidation.Setup(v => v.ValidateExtensionRequest(borrowedBook, 7)).Returns(true);
            _mockReaderService.Setup(s => s.GetReaderById(1)).Returns(reader);

            _borrowedBookService.ExtendBorrowedBook(borrowedBook, 7);

            _mockBorrowedBooksRepository.Verify(r => r.Update(It.IsAny<BorrowedBooks>()), Times.Once);
            _mockExtensionRepository.Verify(r => r.Add(It.IsAny<Extension>()), Times.Once);
            _mockReaderService.Verify(s => s.UpdateReader(It.Is<Reader>(rd => rd.NumberOfExtensions == 1)), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestReturnBook_WithNonExistentRecord_ThrowsException()
        {
            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(999, 999)).Returns((BorrowedBooks)null);

            _borrowedBookService.ReturnBook(999, 999);
        }

        [TestMethod]
        public void TestReturnBook_ValidRequest_DeletesRecordAndUpdatesAvailability()
        {
            var book = new Book { BookId = 1, NumberOfAvailableBooks = 5 };
            var borrowedBook = new BorrowedBooks { BookId = 1, ReaderId = 1 };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(1, 1)).Returns(borrowedBook);
            _mockBookService.Setup(s => s.GetBookById(1)).Returns(book);

            _borrowedBookService.ReturnBook(1, 1);

            _mockBorrowedBooksRepository.Verify(r => r.Delete(1, 1), Times.Once);
            _mockBookService.Verify(s => s.UpdateBook(It.Is<Book>(b => b.NumberOfAvailableBooks == 6)), Times.Once);
        }

        [TestMethod]
        public void TestGetBorrowedBooksByReader_ReturnsCorrectBooks()
        {
            var borrowedBooks = new List<BorrowedBooks>
            {
                new BorrowedBooks { BookId = 1, ReaderId = 1 },
                new BorrowedBooks { BookId = 2, ReaderId = 1 }
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByReader(1)).Returns(borrowedBooks);

            var result = _borrowedBookService.GetBorrowedBooksByReader(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetAllBorrowedBooks_ReturnsAllBooks()
        {
            var borrowedBooks = new List<BorrowedBooks>
            {
                new BorrowedBooks { BookId = 1, ReaderId = 1 },
                new BorrowedBooks { BookId = 2, ReaderId = 2 }
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetAll()).Returns(borrowedBooks);

            var result = _borrowedBookService.GetAllBorrowedBooks();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
