using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer;
using System;
using System.Collections.Generic;
using Moq;
using DataMapper.RepoInterfaces;
using Microsoft.Extensions.Logging;
using FluentValidation;


namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the AuthorService class
    /// </summary>
    [TestClass]
    public class AuthorServiceTests
    {
        private Mock<IAuthorRepository> _mockRepository;
        private Mock<ILogger<AuthorService>> _mockLogger;
        private Mock<IValidator<Author>> _mockValidator;
        private AuthorService _authorService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockRepository = new Mock<IAuthorRepository>();
            _mockLogger = new Mock<ILogger<AuthorService>>();
            _mockValidator = new Mock<IValidator<Author>>();

            _authorService = new AuthorService(
                _mockRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddAuthor_WithNullAuthor_ThrowsException()
        {
            _authorService.AddAuthor(null);
        }

        [TestMethod]
        public void TestAddAuthor_WithValidAuthor_CallsRepositoryAdd()
        {
            var author = new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);

            _mockRepository.Verify(r => r.Add(author), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithEmptyFirstName_ThrowsException()
        {
            var author = new Author
            {
                FirstName = "",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithShortFirstName_ThrowsException()
        {
            var author = new Author
            {
                FirstName = "John",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithLongFirstName_ThrowsException()
        {
            var author = new Author
            {
                FirstName = new string('a', 51),
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithEmptyLastName_ThrowsException()
        {
            var author = new Author
            {
                FirstName = "George",
                LastName = "",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithShortLastName_ThrowsException()
        {
            var author = new Author
            {
                FirstName = "George",
                LastName = "Lee",
                Age = 46,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddAuthor_WithInvalidAge_ThrowsException()
        {
            var author = new Author
            {
                FirstName = "George",
                LastName = "Orwell",
                Age = 5,
                Books = new List<Book>()
            };

            _authorService.AddAuthor(author);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateAuthor_WithNullAuthor_ThrowsException()
        {
            _authorService.UpdateAuthor(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateAuthor_WithNonExistentAuthor_ThrowsException()
        {
            var author = new Author
            {
                AuthorId = 999,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46
            };

            _mockRepository.Setup(r => r.GetById(999)).Returns((Author)null);

            _authorService.UpdateAuthor(author);
        }

        [TestMethod]
        public void TestUpdateAuthor_WithValidAuthor_CallsRepositoryUpdate()
        {
            var author = new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(author);

            _authorService.UpdateAuthor(author);

            _mockRepository.Verify(r => r.Update(author), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetAuthorById_ReturnsCorrectAuthor()
        {
            var author = new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(author);

            var result = _authorService.GetAuthorById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AuthorId);
            Assert.AreEqual("George", result.FirstName);
        }

        [TestMethod]
        public void TestGetAllAuthors_ReturnsAllAuthors()
        {
            var authors = new List<Author>
            {
                new Author { AuthorId = 1, FirstName = "George", LastName = "Orwell", Age = 46 },
                new Author { AuthorId = 2, FirstName = "Isaac", LastName = "Asimov", Age = 72 }
            };

            _mockRepository.Setup(r => r.GetAll()).Returns(authors);

            var result = _authorService.GetAllAuthors();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteAuthor_WithNonExistentAuthor_ThrowsException()
        {
            _mockRepository.Setup(r => r.GetById(999)).Returns((Author)null);

            _authorService.DeleteAuthor(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteAuthor_WithAssociatedBooks_ThrowsException()
        {
            var author = new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>
                {
                    new Book { BookId = 1, Title = "1984" }
                }
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(author);

            _authorService.DeleteAuthor(1);
        }

        [TestMethod]
        public void TestDeleteAuthor_WithValidAuthor_CallsRepositoryDelete()
        {
            var author = new Author
            {
                AuthorId = 1,
                FirstName = "George",
                LastName = "Orwell",
                Age = 46,
                Books = new List<Book>()
            };

            _mockRepository.Setup(r => r.GetById(1)).Returns(author);

            _authorService.DeleteAuthor(1);

            _mockRepository.Verify(r => r.Delete(1), Times.Once);
            _mockRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestGetAllAuthors_WhenRepositoryThrowsException_LogsAndRethrows()
        {
            _mockRepository
                .Setup(r => r.GetAll())
                .Throws(new Exception("Database error"));

            _authorService.GetAllAuthors();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestGetAuthorById_WhenRepositoryThrowsException_LogsAndRethrows()
        {
            _mockRepository
                .Setup(r => r.GetById(It.IsAny<int>()))
                .Throws(new Exception("Database error"));

            _authorService.GetAuthorById(1);
        }


    }
}
