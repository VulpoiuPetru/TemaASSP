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
    /// Tests for the DomainService class
    /// </summary>
    [TestClass]
    public class DomainServiceTests
    {
        private Mock<IDomainRepository> _mockDomainRepository;
        private Mock<ILogger<DomainService>> _mockLogger;
        private Mock<IValidator<Domain>> _mockValidator;
        private DomainService _domainService;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            _mockDomainRepository = new Mock<IDomainRepository>();
            _mockLogger = new Mock<ILogger<DomainService>>();
            _mockValidator = new Mock<IValidator<Domain>>();

            _mockValidator
       .Setup(v => v.Validate(It.IsAny<Domain>()))
       .Returns(new FluentValidation.Results.ValidationResult());

            _domainService = new DomainService(
                _mockDomainRepository.Object,
                _mockLogger.Object,
                _mockValidator.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddDomain_WithNullDomain_ThrowsException()
        {
            _domainService.AddDomain(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDomain_WithEmptyName_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = ""
            };

            _domainService.AddDomain(domain);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDomain_WithShortName_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Test"
            };

            _domainService.AddDomain(domain);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddDomain_WithLongName_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = new string('a', 51)
            };

            _domainService.AddDomain(domain);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddDomain_WithNonExistentParent_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Computer Science",
                Parent = new Domain { DomainId = 999 }
            };

            _mockDomainRepository.Setup(r => r.GetById(999)).Returns((Domain)null);

            _domainService.AddDomain(domain);
        }

        [TestMethod]
        public void TestAddDomain_WithValidDomain_CallsRepositoryAdd()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science"
            };

            _domainService.AddDomain(domain);

            _mockDomainRepository.Verify(r => r.Add(domain), Times.Once);
            _mockDomainRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestAddDomain_WithValidParent_CallsRepositoryAdd()
        {
            var parentDomain = new Domain { DomainId = 1, Name = "Science" };
            var domain = new Domain
            {
                DomainId = 2,
                Name = "Computer Science",
                Parent = parentDomain
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(parentDomain);

            _domainService.AddDomain(domain);

            _mockDomainRepository.Verify(r => r.Add(domain), Times.Once);
            _mockDomainRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestUpdateDomain_WithNullDomain_ThrowsException()
        {
            _domainService.UpdateDomain(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateDomain_WithNonExistentDomain_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 999,
                Name = "Science"
            };

            _mockDomainRepository.Setup(r => r.GetById(999)).Returns((Domain)null);

            _domainService.UpdateDomain(domain);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateDomain_WithSelfAsParent_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science",
                Parent = new Domain { DomainId = 1 }
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);

            _domainService.UpdateDomain(domain);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestUpdateDomain_WithDescendantAsParent_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science",
                Parent = new Domain { DomainId = 2 }
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);
            _mockDomainRepository.Setup(r => r.IsAncestor(1, 2)).Returns(true);

            _domainService.UpdateDomain(domain);
        }

        [TestMethod]
        public void TestUpdateDomain_WithValidDomain_CallsRepositoryUpdate()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Updated Science"
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);

            _domainService.UpdateDomain(domain);

            _mockDomainRepository.Verify(r => r.Update(domain), Times.Once);
            _mockDomainRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetDomainById_ReturnsCorrectDomain()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science"
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);

            var result = _domainService.GetDomainById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.DomainId);
            Assert.AreEqual("Science", result.Name);
        }

        [TestMethod]
        public void TestGetAllDomains_ReturnsAllDomains()
        {
            var domains = new List<Domain>
            {
                new Domain { DomainId = 1, Name = "Science" },
                new Domain { DomainId = 2, Name = "Mathematics" }
            };

            _mockDomainRepository.Setup(r => r.GetAll()).Returns(domains);

            var result = _domainService.GetAllDomains();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteDomain_WithNonExistentDomain_ThrowsException()
        {
            _mockDomainRepository.Setup(r => r.GetById(999)).Returns((Domain)null);

            _domainService.DeleteDomain(999);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteDomain_WithAssociatedBooks_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science",
                Books = new List<Book> { new Book { BookId = 1 } }
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);

            _domainService.DeleteDomain(1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteDomain_WithChildDomains_ThrowsException()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science",
                Books = new List<Book>()
            };

            var descendants = new List<Domain>
            {
                new Domain { DomainId = 2, Name = "Computer Science" }
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);
            _mockDomainRepository.Setup(r => r.GetDescendants(1)).Returns(descendants);

            _domainService.DeleteDomain(1);
        }

        [TestMethod]
        public void TestDeleteDomain_WithValidDomain_CallsRepositoryDelete()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Name = "Science",
                Books = new List<Book>()
            };

            _mockDomainRepository.Setup(r => r.GetById(1)).Returns(domain);
            _mockDomainRepository.Setup(r => r.GetDescendants(1)).Returns(new List<Domain>());

            _domainService.DeleteDomain(1);

            _mockDomainRepository.Verify(r => r.Delete(1), Times.Once);
            _mockDomainRepository.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestGetRootDomains_ReturnsOnlyRootDomains()
        {
            var rootDomains = new List<Domain>
            {
                new Domain { DomainId = 1, Name = "Science", Parent = null },
                new Domain { DomainId = 2, Name = "Arts", Parent = null }
            };

            _mockDomainRepository.Setup(r => r.GetRootDomains()).Returns(rootDomains);

            var result = _domainService.GetRootDomains();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetLeafDomains_ReturnsOnlyLeafDomains()
        {
            var leafDomains = new List<Domain>
            {
                new Domain { DomainId = 3, Name = "Algorithms" },
                new Domain { DomainId = 4, Name = "Databases" }
            };

            _mockDomainRepository.Setup(r => r.GetLeafDomains()).Returns(leafDomains);

            var result = _domainService.GetLeafDomains();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetAncestors_ReturnsAncestorChain()
        {
            var ancestors = new List<Domain>
            {
                new Domain { DomainId = 1, Name = "Science" },
                new Domain { DomainId = 2, Name = "Computer Science" }
            };

            _mockDomainRepository.Setup(r => r.GetAncestors(3)).Returns(ancestors);

            var result = _domainService.GetAncestors(3);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void TestGetDescendants_ReturnsDescendantTree()
        {
            var descendants = new List<Domain>
            {
                new Domain { DomainId = 2, Name = "Computer Science" },
                new Domain { DomainId = 3, Name = "Algorithms" }
            };

            _mockDomainRepository.Setup(r => r.GetDescendants(1)).Returns(descendants);

            var result = _domainService.GetDescendants(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
