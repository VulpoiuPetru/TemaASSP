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
    /// Tests for the CopyService class
    /// </summary>
    [TestClass]
    public class CopyServiceTests
    {
        private Mock<ICopyRepository> _mockCopyRepository;
        private Mock<IEditionRepository> _mockEditionRepository;
        private Mock<ILogger<CopyService>> _mockLogger;
        private Mock<IValidator<Copy>> _mockValidator;
        private CopyService _copyService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockCopyRepository = new Mock<ICopyRepository>();
            _mockEditionRepository = new Mock<IEditionRepository>();
            _mockLogger = new Mock<ILogger<CopyService>>();
            _mockValidator = new Mock<IValidator<Copy>>();

            _mockValidator.Setup(v => v.Validate(It.IsAny<ValidationContext<Copy>>()))
                         .Returns(new FluentValidation.Results.ValidationResult());

            _copyService = new CopyService(
                _mockCopyRepository.Object,
                _mockEditionRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddCopy_WithNullCopy_ThrowsException()
        {
            _copyService.AddCopy(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "No edition assigned")]
        public void TestAddCopy_WithNoEdition_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                IsReadingRoomOnly = false,
                Edition = null
            };

            //_mockEditionRepository.Setup(r => r.GetById(1)).Returns((Edition)null);

            _copyService.AddCopy(copy);  // Ar trebui să arunce ArgumentException din service
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddCopy_WithNonExistentEdition_ThrowsException()
        {
            var edition = new Edition { EditionId = 999 };
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                IsReadingRoomOnly = false,
                Edition = new Edition { EditionId = 999 }
            };

            _mockEditionRepository.Setup(r => r.GetById(999)).Returns((Edition)null);

            _copyService.AddCopy(copy);
        }

        [TestMethod]
        public void TestAddCopy_WithValidCopy_CallsRepositoryAdd()
        {
            var edition = new Edition
            {
                EditionId = 1,
                // Add any other required properties that ValidateCopy might check
                // For example:
                // BookId = 1,
                // PublicationYear = 2020,
                // etc.
            };

            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                IsReadingRoomOnly = false,
                Edition = edition
            };

            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _copyService.AddCopy(copy);

            _mockCopyRepository.Verify(r => r.Add(copy), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateCopy_WithNullCopy_ThrowsException()
        {
            _copyService.UpdateCopy(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateCopy_WithNonExistentCopy_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 999,
                IsAvailable = true,
                Edition = new Edition { EditionId = 1 }
            };

            _mockCopyRepository.Setup(r => r.GetById(999)).Returns((Copy)null);

            _copyService.UpdateCopy(copy);
        }

        [TestMethod]
        public void TestUpdateCopy_WithValidCopy_CallsRepositoryUpdate()
        {
            var edition = new Edition
            {
                EditionId = 1
                // Add any other required properties that ValidateCopy might check
            };

            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                Edition = edition
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);
            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _copyService.UpdateCopy(copy);

            _mockCopyRepository.Verify(r => r.Update(copy), Times.Once);
        }

        [TestMethod]
        public void TestGetCopyById_ReturnsCorrectCopy()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            var result = _copyService.GetCopyById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.IsTrue(result.IsAvailable);
        }

        [TestMethod]
        public void TestGetAllCopies_ReturnsAllCopies()
        {
            var copies = new List<Copy>
            {
                new Copy { Id = 1, IsAvailable = true },
                new Copy { Id = 2, IsAvailable = false }
            };

            _mockCopyRepository.Setup(r => r.GetAll()).Returns(copies);

            var result = _copyService.GetAllCopies();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteCopy_WithNonExistentCopy_ThrowsException()
        {
            _mockCopyRepository.Setup(r => r.GetById(999)).Returns((Copy)null);

            _copyService.DeleteCopy(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteCopy_WithBorrowedCopy_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = false
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            _copyService.DeleteCopy(1);
        }

        [TestMethod]
        public void TestDeleteCopy_WithValidCopy_CallsRepositoryDelete()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            _copyService.DeleteCopy(1);

            _mockCopyRepository.Verify(r => r.Delete(1), Times.Once);
        }

        [TestMethod]
        public void TestGetByEditionId_ReturnsCorrectCopies()
        {
            var copies = new List<Copy>
            {
                new Copy { Id = 1, Edition = new Edition { EditionId = 1 } },
                new Copy { Id = 2, Edition = new Edition { EditionId = 1 } }
            };

            _mockCopyRepository.Setup(r => r.GetByEditionId(1)).Returns(copies);

            var result = _copyService.GetByEditionId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetAvailableBorrowableCopies_ReturnsOnlyAvailableNonReadingRoom()
        {
            var copies = new List<Copy>
            {
                new Copy { Id = 1, IsAvailable = true, IsReadingRoomOnly = false }
            };

            _mockCopyRepository.Setup(r => r.GetAvailableBorrowableCopies(1)).Returns(copies);

            var result = _copyService.GetAvailableBorrowableCopies(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result[0].IsAvailable);
            Assert.IsFalse(result[0].IsReadingRoomOnly);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMarkAsBorrowed_WithNonExistentCopy_ThrowsException()
        {
            _mockCopyRepository.Setup(r => r.GetById(999)).Returns((Copy)null);

            _copyService.MarkAsBorrowed(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMarkAsBorrowed_WithAlreadyBorrowedCopy_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = false
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            _copyService.MarkAsBorrowed(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMarkAsBorrowed_WithReadingRoomOnly_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                IsReadingRoomOnly = true
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            _copyService.MarkAsBorrowed(1);
        }

        [TestMethod]
        public void TestMarkAsBorrowed_WithValidCopy_UpdatesAvailability()
        {
            var edition = new Edition
            {
                EditionId = 1
                // Add other required properties
            };

            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true,
                IsReadingRoomOnly = false,
                Edition = edition
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);
            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _copyService.MarkAsBorrowed(1);

            Assert.IsFalse(copy.IsAvailable);
            _mockCopyRepository.Verify(r => r.Update(copy), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMarkAsReturned_WithNonExistentCopy_ThrowsException()
        {
            _mockCopyRepository.Setup(r => r.GetById(999)).Returns((Copy)null);

            _copyService.MarkAsReturned(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestMarkAsReturned_WithAlreadyAvailableCopy_ThrowsException()
        {
            var copy = new Copy
            {
                Id = 1,
                IsAvailable = true
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);

            _copyService.MarkAsReturned(1);
        }

        [TestMethod]
        public void TestMarkAsReturned_WithValidCopy_UpdatesAvailability()
        {
            var edition = new Edition
            {
                EditionId = 1
                // Add other required properties
            };

            var copy = new Copy
            {
                Id = 1,
                IsAvailable = false,
                Edition = edition
            };

            _mockCopyRepository.Setup(r => r.GetById(1)).Returns(copy);
            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _copyService.MarkAsReturned(1);

            Assert.IsTrue(copy.IsAvailable);
            _mockCopyRepository.Verify(r => r.Update(copy), Times.Once);
        }
    }
}
