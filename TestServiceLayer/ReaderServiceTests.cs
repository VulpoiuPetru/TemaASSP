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
    /// Tests for the ReaderService class
    /// </summary>
    [TestClass]
    public class ReaderServiceTests
    {
        private Mock<IReaderRepository> _mockReaderRepository;
        private Mock<ILogger<ReaderService>> _mockLogger;
        private Mock<IValidator<Reader>> _mockValidator;
        private ReaderService _readerService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockReaderRepository = new Mock<IReaderRepository>();
            _mockLogger = new Mock<ILogger<ReaderService>>();
            _mockValidator = new Mock<IValidator<Reader>>();

            _mockValidator
        .Setup(v => v.Validate(It.IsAny<Reader>()))
        .Returns(new FluentValidation.Results.ValidationResult());

            _readerService = new ReaderService(
                _mockReaderRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddReader_WithNullReader_ThrowsException()
        {
            _readerService.AddReader(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithEmptyFirstName_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com"
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithEmptyLastName_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "",
                Age = 25,
                Email = "test@test.com"
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithAgeTooLow_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 5,
                Email = "test@test.com"
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithAgeTooHigh_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 85,
                Email = "test@test.com"
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithNoContactInfo_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = null,
                PhoneNumber = null
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithInvalidEmail_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "invalid"
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddReader_WithNegativeExtensions_ThrowsException()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com",
                NumberOfExtensions = -1
            };

            _readerService.AddReader(reader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddReader_WithDuplicateEmail_ThrowsException()
        {
            var existingReader = new Reader
            {
                ReaderId = 1,
                Email = "test@test.com"
            };

            var newReader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetByEmail("test@test.com")).Returns(existingReader);

            _readerService.AddReader(newReader);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddReader_WithDuplicatePhoneNumber_ThrowsException()
        {
            var existingReader = new Reader
            {
                ReaderId = 1,
                PhoneNumber = "1234567890"
            };

            var newReader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                PhoneNumber = "1234567890"
            };

            _mockReaderRepository.Setup(r => r.GetByPhoneNumber("1234567890")).Returns(existingReader);

            _readerService.AddReader(newReader);
        }

        [TestMethod]
        public void TestAddReader_WithValidReader_CallsRepositoryAdd()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Reader)null);
            _mockReaderRepository.Setup(r => r.GetByPhoneNumber(It.IsAny<string>())).Returns((Reader)null);

            _readerService.AddReader(reader);

            _mockReaderRepository.Verify(r => r.Add(reader), Times.Once);
            _mockReaderRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateReader_WithNullReader_ThrowsException()
        {
            _readerService.UpdateReader(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateReader_WithNonExistentReader_ThrowsException()
        {
            var reader = new Reader
            {
                ReaderId = 999,
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetById(999)).Returns((Reader)null);

            _readerService.UpdateReader(reader);
        }

        [TestMethod]
        public void TestUpdateReader_WithValidReader_CallsRepositoryUpdate()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "test@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);
            _mockReaderRepository.Setup(r => r.GetByEmail("test@test.com")).Returns(reader);

            _readerService.UpdateReader(reader);

            _mockReaderRepository.Verify(r => r.Update(reader), Times.Once);
            _mockReaderRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetReaderById_ReturnsCorrectReader()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith"
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);

            var result = _readerService.GetReaderById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ReaderId);
            Assert.AreEqual("John", result.FirstName);
        }

        [TestMethod]
        public void TestGetAllReaders_ReturnsAllReaders()
        {
            var readers = new List<Reader>
            {
                new Reader { ReaderId = 1, FirstName = "John", LastName = "Smith" },
                new Reader { ReaderId = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _mockReaderRepository.Setup(r => r.GetAll()).Returns(readers);

            var result = _readerService.GetAllReaders();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteReader_WithNonExistentReader_ThrowsException()
        {
            _mockReaderRepository.Setup(r => r.GetById(999)).Returns((Reader)null);

            _readerService.DeleteReader(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteReader_WithActiveBorrows_ThrowsException()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                BorrowedBooks = new List<BorrowedBooks>
                {
                    new BorrowedBooks { BorrowEndDate = DateTime.Now.AddDays(7) }
                }
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);

            _readerService.DeleteReader(1);
        }

        [TestMethod]
        public void TestDeleteReader_WithValidReader_CallsRepositoryDelete()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                BorrowedBooks = new List<BorrowedBooks>()
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);

            _readerService.DeleteReader(1);

            _mockReaderRepository.Verify(r => r.Delete(1), Times.Once);
            _mockReaderRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestAddReader_WithValidEmail_Succeeds()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "john@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetByEmail(It.IsAny<string>())).Returns((Reader)null);

            _readerService.AddReader(reader);

            _mockReaderRepository.Verify(r => r.Add(reader), Times.Once);
        }

        [TestMethod]
        public void TestAddReader_WithValidPhoneNumber_Succeeds()
        {
            var reader = new Reader
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                PhoneNumber = "1234567890"
            };

            _mockReaderRepository.Setup(r => r.GetByPhoneNumber(It.IsAny<string>())).Returns((Reader)null);

            _readerService.AddReader(reader);

            _mockReaderRepository.Verify(r => r.Add(reader), Times.Once);
        }

        [TestMethod]
        public void TestUpdateReader_AllowsSameReaderEmail_Succeeds()
        {
            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "john@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);
            _mockReaderRepository.Setup(r => r.GetByEmail("john@test.com")).Returns(reader);

            _readerService.UpdateReader(reader);

            _mockReaderRepository.Verify(r => r.Update(reader), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateReader_WithAnotherReaderEmail_ThrowsException()
        {
            var existingReader = new Reader
            {
                ReaderId = 2,
                Email = "john@test.com"
            };

            var reader = new Reader
            {
                ReaderId = 1,
                FirstName = "John",
                LastName = "Smith",
                Age = 25,
                Email = "john@test.com"
            };

            _mockReaderRepository.Setup(r => r.GetById(1)).Returns(reader);
            _mockReaderRepository.Setup(r => r.GetByEmail("john@test.com")).Returns(existingReader);

            _readerService.UpdateReader(reader);
        }
    }
}
