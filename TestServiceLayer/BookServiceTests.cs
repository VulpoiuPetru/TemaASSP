using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceLayer;
using ServiceLayer.ServiceConfiguration;
using System;
using System.Collections.Generic;

namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the BookService class
    /// </summary>
    [TestClass]
    public class BookServiceTests
    {
        private Mock<IConfigurationService> _mockConfigService;
        private Mock<IBookRepository> _mockBookRepository;
        private Mock<IDomainRepository> _mockDomainRepository;
        private Mock<ILogger<BookService>> _mockLogger;
        private Mock<IValidator<Book>> _mockValidator;
        private BookService _bookService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockConfigService = new Mock<IConfigurationService>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockDomainRepository = new Mock<IDomainRepository>();
            _mockLogger = new Mock<ILogger<BookService>>();
            _mockValidator = new Mock<IValidator<Book>>();

            _bookService = new BookService(
                _mockConfigService.Object,
                _mockBookRepository.Object,
                _mockDomainRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);

            var config = new LibraryConfiguration { DOMENII = 3 };
            _mockConfigService.Setup(c => c.GetConfiguration()).Returns(config);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddBook_WithNullBook_ThrowsException()
        {
            _bookService.AddBook(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithEmptyTitle_ThrowsException()
        {
            var book = new Book
            {
                Title = "",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition()
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithNoAuthors_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author>(),
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition()
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithNoDomains_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain>(),
                Edition = new Edition()
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithNegativeTotalBooks_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfTotalBooks = -1
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithNegativeReadingRoomBooks_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfReadingRoomBooks = -1
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithReadingRoomBooksExceedingTotal_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfTotalBooks = 5,
                NumberOfReadingRoomBooks = 10
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithAvailableBooksExceedingTotal_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfTotalBooks = 5,
                NumberOfAvailableBooks = 10
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBook_WithNoEdition_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = null
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddBook_WithTooManyDomains_ThrowsException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain>
                {
                    new Domain { DomainId = 1 },
                    new Domain { DomainId = 2 },
                    new Domain { DomainId = 3 },
                    new Domain { DomainId = 4 }
                },
                Edition = new Edition(),
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10
            };

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddBook_WithAncestorDescendantDomains_ThrowsException()
        {
            var domain1 = new Domain { DomainId = 1, Name = "Science" };
            var domain2 = new Domain { DomainId = 2, Name = "Computer Science" };

            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { domain1, domain2 },
                Edition = new Edition(),
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10
            };

            _mockDomainRepository.Setup(r => r.IsAncestor(1, 2)).Returns(true);

            _bookService.AddBook(book);
        }

        [TestMethod]
        public void TestAddBook_WithValidBook_CallsRepositoryAdd()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                Authors = new List<Author> { new Author { AuthorId = 1 } },
                Domains = new List<Domain> { new Domain { DomainId = 1 } },
                Edition = new Edition { EditionId = 1 },
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10,
                NumberOfReadingRoomBooks = 0
            };

            _mockDomainRepository.Setup(r => r.IsAncestor(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            _bookService.AddBook(book);

            _mockBookRepository.Verify(r => r.Add(book), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateBook_WithNullBook_ThrowsException()
        {
            _bookService.UpdateBook(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateBook_WithNonExistentBook_ThrowsException()
        {
            var book = new Book
            {
                BookId = 999,
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition()
            };

            _mockBookRepository.Setup(r => r.GetById(999)).Returns((Book)null);

            _bookService.UpdateBook(book);
        }

        [TestMethod]
        public void TestUpdateBook_WithValidBook_CallsRepositoryUpdate()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Updated Book",
                Authors = new List<Author> { new Author { AuthorId = 1 } },
                Domains = new List<Domain> { new Domain { DomainId = 1 } },
                Edition = new Edition { EditionId = 1 },
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10,
                NumberOfReadingRoomBooks = 0
            };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);
            _mockDomainRepository.Setup(r => r.IsAncestor(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            _bookService.UpdateBook(book);

            _mockBookRepository.Verify(r => r.Update(book), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetBookById_ReturnsCorrectBook()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book"
            };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            var result = _bookService.GetBookById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BookId);
            Assert.AreEqual("Test Book", result.Title);
        }

        [TestMethod]
        public void TestGetAllBooks_ReturnsAllBooks()
        {
            var books = new List<Book>
            {
                new Book { BookId = 1, Title = "Book 1" },
                new Book { BookId = 2, Title = "Book 2" }
            };

            _mockBookRepository.Setup(r => r.GetAll()).Returns(books);

            var result = _bookService.GetAllBooks();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteBook_WithNonExistentBook_ThrowsException()
        {
            _mockBookRepository.Setup(r => r.GetById(999)).Returns((Book)null);

            _bookService.DeleteBook(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteBook_WithActiveBorrows_ThrowsException()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                BorrowedBooks = new List<BorrowedBooks>
                {
                    new BorrowedBooks { BorrowEndDate = DateTime.Now.AddDays(7) }
                }
            };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            _bookService.DeleteBook(1);
        }

        [TestMethod]
        public void TestDeleteBook_WithValidBook_CallsRepositoryDelete()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            _bookService.DeleteBook(1);

            _mockBookRepository.Verify(r => r.Delete(1), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSetBookDomains_WithNonExistentBook_ThrowsException()
        {
            _mockBookRepository.Setup(r => r.GetById(999)).Returns((Book)null);

            _bookService.SetBookDomains(999, new List<int> { 1 });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSetBookDomains_WithEmptyDomainList_ThrowsException()
        {
            var book = new Book { BookId = 1, Domains = new List<Domain>() };
            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            _bookService.SetBookDomains(1, new List<int>());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSetBookDomains_WithTooManyDomains_ThrowsException()
        {
            var book = new Book { BookId = 1, Domains = new List<Domain>() };
            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            _bookService.SetBookDomains(1, new List<int> { 1, 2, 3, 4 });
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSetBookDomains_WithAncestorDescendant_ThrowsException()
        {
            var book = new Book { BookId = 1, Domains = new List<Domain>() };
            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);
            _mockDomainRepository.Setup(r => r.IsAncestor(1, 2)).Returns(true);

            _bookService.SetBookDomains(1, new List<int> { 1, 2 });
        }

        [TestMethod]
        public void TestSetBookDomains_WithValidDomains_UpdatesBook()
        {
            var book = new Book { BookId = 1, Domains = new List<Domain>() };
            var domain1 = new Domain { DomainId = 1, Name = "Domain 1" };
            var domain2 = new Domain { DomainId = 2, Name = "Domain 2" };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);
            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain1);
            _mockDomainRepository.Setup(r => r.GetById(2)).Returns(domain2);
            _mockDomainRepository.Setup(r => r.IsAncestor(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            _bookService.SetBookDomains(1, new List<int> { 1, 2 });

            _mockBookRepository.Verify(r => r.Update(book), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}
