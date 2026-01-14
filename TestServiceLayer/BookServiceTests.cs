using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using FluentValidation.Results;
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

        [TestInitialize]
        public void SetUp()
        {
            _mockConfigService = new Mock<IConfigurationService>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockDomainRepository = new Mock<IDomainRepository>();
            _mockLogger = new Mock<ILogger<BookService>>();
            _mockValidator = new Mock<IValidator<Book>>();

            // Implicit: toate cărțile sunt valide (ValidationResult fără erori)
            _mockValidator
                .Setup(v => v.Validate(It.IsAny<Book>()))
                .Returns(new ValidationResult());

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
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithNoAuthors_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author>(),
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                BorrowedBooks = new List<BorrowedBooks>()
            };

            // Simulează eroare de validare (fără autori)
            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("Authors", "Book must have at least one author")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithNoDomains_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain>(),
                Edition = new Edition(),
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("Domains", "Book must belong to at least one domain")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithNegativeTotalBooks_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfTotalBooks = -1,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("NumberOfTotalBooks", "Total books cannot be negative")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithNegativeReadingRoomBooks_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfReadingRoomBooks = -1,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("NumberOfReadingRoomBooks", "Reading room books cannot be negative")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithReadingRoomBooksExceedingTotal_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = new Edition(),
                NumberOfTotalBooks = 5,
                NumberOfReadingRoomBooks = 10,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("NumberOfReadingRoomBooks",
                        "Reading room books cannot exceed total books")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithAvailableBooksExceedingTotal_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author { FirstName = "John", LastName = "Doe", Age = 40 } },
                Domains = new List<Domain> { new Domain { DomainId = 1, Name = "Science" } },
                Edition = new Edition { EditionId = 1 },
                NumberOfTotalBooks = 5,
                NumberOfAvailableBooks = 10,
                NumberOfReadingRoomBooks = 0,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("NumberOfAvailableBooks",
                        "Available books cannot exceed total books")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithNoEdition_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "Test Book",
                Authors = new List<Author> { new Author() },
                Domains = new List<Domain> { new Domain() },
                Edition = null,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("Edition", "Edition is required")
                }));

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
                NumberOfAvailableBooks = 10,
                BorrowedBooks = new List<BorrowedBooks>()
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
                NumberOfAvailableBooks = 10,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockDomainRepository.Setup(r => r.IsAncestor(1, 2)).Returns(true);

            _bookService.AddBook(book);
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void TestAddBook_WithEmptyTitle_ThrowsValidationException()
        {
            var book = new Book
            {
                Title = "",
                Authors = new List<Author> { new Author { FirstName = "Test", LastName = "Author", Age = 30 } },
                Domains = new List<Domain> { new Domain { DomainId = 1, Name = "Test" } },
                Edition = new Edition { EditionId = 1 },
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10,
                NumberOfReadingRoomBooks = 0,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockValidator
                .Setup(v => v.Validate(book))
                .Returns(new ValidationResult(new[]
                {
                    new ValidationFailure("Title", "Title is required")
                }));

            _bookService.AddBook(book);
        }

        [TestMethod]
        public void TestAddBook_WithValidBook_CallsRepositoryAdd()
        {
            var book = new Book
            {
                BookId = 1,
                Title = "Test Book",
                Authors = new List<Author> { new Author { AuthorId = 1, FirstName = "John", LastName = "Doe", Age = 40 } },
                Domains = new List<Domain> { new Domain { DomainId = 1, Name = "Science" } },
                Edition = new Edition { EditionId = 1, Publisher = "TestPub" },
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 10,
                NumberOfReadingRoomBooks = 0,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockDomainRepository.Setup(r => r.IsAncestor(It.IsAny<int>(), It.IsAny<int>())).Returns(false);

            // implicit validator valid (din SetUp)
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
                Edition = new Edition(),
                BorrowedBooks = new List<BorrowedBooks>()
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
                NumberOfReadingRoomBooks = 0,
                BorrowedBooks = new List<BorrowedBooks>()
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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddBook_AncestorDescendantDomainsAssigned_Throws()
        {
            var configService = new Mock<IConfigurationService>();
            configService.Setup(c => c.GetConfiguration()).Returns(new LibraryConfiguration { DOMENII = 5 });
            var bookRepo = new Mock<IBookRepository>();
            var domainRepo = new Mock<IDomainRepository>();
            var logger = new Mock<ILogger<BookService>>();
            var validator = new Mock<IValidator<Book>>();

            validator
                .Setup(v => v.Validate(It.IsAny<Book>()))
                .Returns(new ValidationResult());

            var service = new BookService(configService.Object, bookRepo.Object, domainRepo.Object, logger.Object, validator.Object);

            var root = new Domain { DomainId = 10, Name = "Science" };
            var child = new Domain { DomainId = 11, Name = "Computer Science", Parent = root };

            var b = new Book
            {
                BookId = 2,
                Title = "Yyyyy",
                Edition = new Edition { EditionId = 2, Publisher = "Publi", NumberOfPages = 3, YearOfPublishing = 2020, Type = "Hardcover" },
                Authors = new List<Author> { new Author { AuthorId = 2, FirstName = "Ccccc", LastName = "Ddddd", Age = 40 } },
                Domains = new List<Domain> { root, child },
                NumberOfTotalBooks = 10,
                NumberOfAvailableBooks = 9,
                NumberOfReadingRoomBooks = 1,
                BorrowedBooks = new List<BorrowedBooks>()
            };

            domainRepo.Setup(d => d.IsAncestor(10, 11)).Returns(true);

            service.AddBook(b);
        }
    }
}
