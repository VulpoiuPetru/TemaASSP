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
    /// Tests for the EditionService class
    /// </summary>
    [TestClass]
    public class EditionServiceTests
    {
        private Mock<IEditionRepository> _mockEditionRepository;
        private Mock<IBookRepository> _mockBookRepository;
        private Mock<ILogger<EditionService>> _mockLogger;
        private Mock<IValidator<Edition>> _mockValidator;
        private EditionService _editionService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockEditionRepository = new Mock<IEditionRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockLogger = new Mock<ILogger<EditionService>>();
            _mockValidator = new Mock<IValidator<Edition>>();

            _mockValidator
        .Setup(v => v.Validate(It.IsAny<Edition>()))
        .Returns(new FluentValidation.Results.ValidationResult());

            _editionService = new EditionService(
                _mockEditionRepository.Object,
                _mockBookRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddEdition_WithNullEdition_ThrowsException()
        {
            _editionService.AddEdition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithEmptyPublisher_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = new Book()
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithShortPublisher_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = new Book()
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithTooFewPages_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 2,
                Type = "Paperback",
                Book = new Book()
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithInvalidYear_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test Publisher",
                YearOfPublishing = 999,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = new Book()
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithEmptyType_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "",
                Book = new Book()
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddEdition_WithNoBook_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = null
            };

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddEdition_WithNonExistentBook_ThrowsException()
        {
            var edition = new Edition
            {
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = new Book { BookId = 999 }
            };

            _mockBookRepository.Setup(r => r.GetById(999)).Returns((Book)null);

            _editionService.AddEdition(edition);
        }

        [TestMethod]
        public void TestAddEdition_WithValidEdition_CallsRepositoryAdd()
        {
            var book = new Book { BookId = 1 };
            var edition = new Edition
            {
                EditionId = 1,
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = book
            };

            _mockBookRepository.Setup(r => r.GetById(1)).Returns(book);

            _editionService.AddEdition(edition);

            _mockEditionRepository.Verify(r => r.Add(edition), Times.Once);
            _mockEditionRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateEdition_WithNullEdition_ThrowsException()
        {
            _editionService.UpdateEdition(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateEdition_WithNonExistentEdition_ThrowsException()
        {
            var edition = new Edition
            {
                EditionId = 999,
                Publisher = "Test Publisher",
                YearOfPublishing = 2020,
                NumberOfPages = 200,
                Type = "Paperback",
                Book = new Book()
            };

            _mockEditionRepository.Setup(r => r.GetById(999)).Returns((Edition)null);

            _editionService.UpdateEdition(edition);
        }

        [TestMethod]
        public void TestUpdateEdition_WithValidEdition_CallsRepositoryUpdate()
        {
            var edition = new Edition
            {
                EditionId = 1,
                Publisher = "Updated Publisher",
                YearOfPublishing = 2021,
                NumberOfPages = 250,
                Type = "Hardcover",
                Book = new Book()
            };

            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _editionService.UpdateEdition(edition);

            _mockEditionRepository.Verify(r => r.Update(edition), Times.Once);
            _mockEditionRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetEditionById_ReturnsCorrectEdition()
        {
            var edition = new Edition
            {
                EditionId = 1,
                Publisher = "Test Publisher",
                YearOfPublishing = 2020
            };

            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            var result = _editionService.GetEditionById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.EditionId);
            Assert.AreEqual("Test Publisher", result.Publisher);
        }

        [TestMethod]
        public void TestGetAllEditions_ReturnsAllEditions()
        {
            var editions = new List<Edition>
            {
                new Edition { EditionId = 1, Publisher = "Publisher A" },
                new Edition { EditionId = 2, Publisher = "Publisher B" }
            };

            _mockEditionRepository.Setup(r => r.GetAll()).Returns(editions);

            var result = _editionService.GetAllEditions();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteEdition_WithNonExistentEdition_ThrowsException()
        {
            _mockEditionRepository.Setup(r => r.GetById(999)).Returns((Edition)null);

            _editionService.DeleteEdition(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteEdition_WithAssociatedCopies_ThrowsException()
        {
            var edition = new Edition
            {
                EditionId = 1,
                Publisher = "Test Publisher",
                Copies = new List<Copy> { new Copy { Id = 1 } }
            };

            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _editionService.DeleteEdition(1);
        }

        [TestMethod]
        public void TestDeleteEdition_WithValidEdition_CallsRepositoryDelete()
        {
            var edition = new Edition
            {
                EditionId = 1,
                Publisher = "Test Publisher",
                Copies = new List<Copy>()
            };

            _mockEditionRepository.Setup(r => r.GetById(1)).Returns(edition);

            _editionService.DeleteEdition(1);

            _mockEditionRepository.Verify(r => r.Delete(1), Times.Once);
            _mockEditionRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetByPublisher_WithEmptyPublisher_ThrowsException()
        {
            _editionService.GetByPublisher("");
        }

        [TestMethod]
        public void TestGetByPublisher_ReturnsCorrectEditions()
        {
            var editions = new List<Edition>
            {
                new Edition { EditionId = 1, Publisher = "Test Publisher" },
                new Edition { EditionId = 2, Publisher = "Test Publisher" }
            };

            _mockEditionRepository.Setup(r => r.GetByPublisher("Test Publisher")).Returns(editions);

            var result = _editionService.GetByPublisher("Test Publisher");

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetByYear_WithInvalidYear_ThrowsException()
        {
            _editionService.GetByYear(999);
        }

        [TestMethod]
        public void TestGetByYear_ReturnsCorrectEditions()
        {
            var editions = new List<Edition>
            {
                new Edition { EditionId = 1, YearOfPublishing = 2020 },
                new Edition { EditionId = 2, YearOfPublishing = 2020 }
            };

            _mockEditionRepository.Setup(r => r.GetByYear(2020)).Returns(editions);

            var result = _editionService.GetByYear(2020);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
