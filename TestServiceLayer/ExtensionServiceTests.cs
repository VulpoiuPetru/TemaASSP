using DataMapper.RepoInterfaces;
using DomainModel;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ServiceLayer;
using System;
using System.Collections.Generic;

namespace TestServiceLayer
{

    /// <summary>
    /// Tests for the ExtensionService class
    /// </summary>
    [TestClass]
    public class ExtensionServiceTests
    {
        private Mock<IExtensionRepository> _mockExtensionRepository;
        private Mock<IBorrowedBooksRepository> _mockBorrowedBooksRepository;
        private Mock<ILogger<ExtensionService>> _mockLogger;
        private Mock<IValidator<Extension>> _mockValidator;
        private ExtensionService _extensionService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockExtensionRepository = new Mock<IExtensionRepository>();
            _mockBorrowedBooksRepository = new Mock<IBorrowedBooksRepository>();
            _mockLogger = new Mock<ILogger<ExtensionService>>();
            _mockValidator = new Mock<IValidator<Extension>>();

            _extensionService = new ExtensionService(
                _mockExtensionRepository.Object,
                _mockBorrowedBooksRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddExtension_WithNullExtension_ThrowsException()
        {
            _extensionService.AddExtension(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithZeroBookId_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 0,
                ReaderId = 1,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithZeroReaderId_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 1,
                ReaderId = 0,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithZeroExtensionDays_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 0,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithTooManyDays_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 100,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithMinDateRequestDate_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 7,
                RequestDate = DateTime.MinValue,
                BorrowedBooks = new BorrowedBooks()
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddExtension_WithNoBorrowedBook_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = null
            };

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddExtension_WithNonExistentBorrowedBook_ThrowsException()
        {
            var extension = new Extension
            {
                BookId = 999,
                ReaderId = 999,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(999, 999)).Returns((BorrowedBooks)null);

            _extensionService.AddExtension(extension);
        }

        [TestMethod]
        public void TestAddExtension_WithValidExtension_CallsRepositoryAdd()
        {
            var borrowedBook = new BorrowedBooks { BookId = 1, ReaderId = 1 };
            var extension = new Extension
            {
                ExtensionId = 1,
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = borrowedBook
            };

            _mockBorrowedBooksRepository.Setup(r => r.GetByIds(1, 1)).Returns(borrowedBook);

            _extensionService.AddExtension(extension);

            _mockExtensionRepository.Verify(r => r.Add(extension), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateExtension_WithNullExtension_ThrowsException()
        {
            _extensionService.UpdateExtension(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateExtension_WithNonExistentExtension_ThrowsException()
        {
            var extension = new Extension
            {
                ExtensionId = 999,
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 7,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _mockExtensionRepository.Setup(r => r.GetById(999)).Returns((Extension)null);

            _extensionService.UpdateExtension(extension);
        }

        [TestMethod]
        public void TestUpdateExtension_WithValidExtension_CallsRepositoryUpdate()
        {
            var extension = new Extension
            {
                ExtensionId = 1,
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 10,
                RequestDate = DateTime.Now,
                BorrowedBooks = new BorrowedBooks()
            };

            _mockExtensionRepository.Setup(r => r.GetById(1)).Returns(extension);

            _extensionService.UpdateExtension(extension);

            _mockExtensionRepository.Verify(r => r.Update(extension), Times.Once);
        }

        [TestMethod]
        public void TestGetExtensionById_ReturnsCorrectExtension()
        {
            var extension = new Extension
            {
                ExtensionId = 1,
                BookId = 1,
                ReaderId = 1,
                ExtensionDays = 7
            };

            _mockExtensionRepository.Setup(r => r.GetById(1)).Returns(extension);

            var result = _extensionService.GetExtensionById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ExtensionId);
            Assert.AreEqual(7, result.ExtensionDays);
        }

        [TestMethod]
        public void TestGetAllExtensions_ReturnsAllExtensions()
        {
            var extensions = new List<Extension>
            {
                new Extension { ExtensionId = 1, BookId = 1, ReaderId = 1 },
                new Extension { ExtensionId = 2, BookId = 2, ReaderId = 2 }
            };

            _mockExtensionRepository.Setup(r => r.GetAll()).Returns(extensions);

            var result = _extensionService.GetAllExtensions();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteExtension_WithNonExistentExtension_ThrowsException()
        {
            _mockExtensionRepository.Setup(r => r.GetById(999)).Returns((Extension)null);

            _extensionService.DeleteExtension(999);
        }

        [TestMethod]
        public void TestDeleteExtension_WithValidExtension_CallsRepositoryDelete()
        {
            var extension = new Extension
            {
                ExtensionId = 1,
                BookId = 1,
                ReaderId = 1
            };

            _mockExtensionRepository.Setup(r => r.GetById(1)).Returns(extension);

            _extensionService.DeleteExtension(1);

            _mockExtensionRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        public void TestGetByReaderId_ReturnsCorrectExtensions()
        {
            var extensions = new List<Extension>
            {
                new Extension { ExtensionId = 1, ReaderId = 1 },
                new Extension { ExtensionId = 2, ReaderId = 1 }
            };

            _mockExtensionRepository.Setup(r => r.GetByReaderId(1)).Returns(extensions);

            var result = _extensionService.GetByReaderId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetByBookId_ReturnsCorrectExtensions()
        {
            var extensions = new List<Extension>
            {
                new Extension { ExtensionId = 1, BookId = 1 },
                new Extension { ExtensionId = 2, BookId = 1 }
            };

            _mockExtensionRepository.Setup(r => r.GetByBookId(1)).Returns(extensions);

            var result = _extensionService.GetByBookId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetTotalExtensionDaysForReaderInLastMonths_WithZeroMonths_ThrowsException()
        {
            _extensionService.GetTotalExtensionDaysForReaderInLastMonths(1, 0);
        }

        [TestMethod]
        public void TestGetTotalExtensionDaysForReaderInLastMonths_ReturnsCorrectTotal()
        {
            _mockExtensionRepository.Setup(r => r.GetTotalExtensionDaysForReaderInLastMonths(1, 3)).Returns(21);

            var result = _extensionService.GetTotalExtensionDaysForReaderInLastMonths(1, 3);

            Assert.AreEqual(21, result);
        }
    }
}
