using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{
    /// <summary>
    /// Tests for the Domain domain model class
    /// </summary>
    [TestClass]
    public class DomainTests
    {
        private Domain domain;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.domain = new Domain
            {
                DomainId = 1,
                Name = "Science"
            };
        }

        [TestMethod]
        public void TestCorrectDomain()
        {
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestNullName()
        {
            this.domain.Name = null;
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestNameTooShort()
        {
            this.domain.Name = "Math";
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestNameMinimumLength()
        {
            this.domain.Name = "Music";
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestNameTooLong()
        {
            this.domain.Name = new string('a', 51);
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestNameMaximumLength()
        {
            this.domain.Name = new string('a', 50);
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestEmptyName()
        {
            this.domain.Name = string.Empty;
            var context = new ValidationContext(this.domain, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.domain, context, results, true));
        }

        [TestMethod]
        public void TestParentDomainNull()
        {
            this.domain.Parent = null;
            Assert.IsNull(this.domain.Parent);
        }

        [TestMethod]
        public void TestParentDomainNotNull()
        {
            var parentDomain = new Domain
            {
                DomainId = 2,
                Name = "Technology"
            };
            this.domain.Parent = parentDomain;
            Assert.IsNotNull(this.domain.Parent);
            Assert.AreEqual("Technology", this.domain.Parent.Name);
        }

        [TestMethod]
        public void TestDomainIdCanBeSet()
        {
            this.domain.DomainId = 100;
            Assert.AreEqual(100, this.domain.DomainId);
        }

        [TestMethod]
        public void TestSubdomainsCollectionInitialized()
        {
            var newDomain = new Domain();
            Assert.IsNotNull(newDomain.Subdomains);
            Assert.AreEqual(0, newDomain.Subdomains.Count);
        }

        [TestMethod]
        public void TestBooksCollectionInitialized()
        {
            var newDomain = new Domain();
            Assert.IsNotNull(newDomain.Books);
            Assert.AreEqual(0, newDomain.Books.Count);
        }

        [TestMethod]
        public void TestDomainWithSubdomains()
        {
            var subdomain1 = new Domain { DomainId = 2, Name = "Mathematics", Parent = this.domain };
            var subdomain2 = new Domain { DomainId = 3, Name = "Physics", Parent = this.domain };

            this.domain.Subdomains.Add(subdomain1);
            this.domain.Subdomains.Add(subdomain2);

            Assert.AreEqual(2, this.domain.Subdomains.Count);
        }

        [TestMethod]
        public void TestDomainHierarchy()
        {
            var parentDomain = new Domain { DomainId = 1, Name = "Science" };
            var childDomain = new Domain { DomainId = 2, Name = "Physics", Parent = parentDomain };
            var grandchildDomain = new Domain { DomainId = 3, Name = "Quantum Physics", Parent = childDomain };

            Assert.AreEqual(parentDomain.DomainId, childDomain.Parent.DomainId);
            Assert.AreEqual(childDomain.DomainId, grandchildDomain.Parent.DomainId);
        }

        [TestMethod]
        public void TestGetAllAncestorsForLeafDomain()
        {
            var root = new Domain { DomainId = 1, Name = "Science" };
            var child = new Domain { DomainId = 2, Name = "Computer Science", Parent = root };
            var grandchild = new Domain { DomainId = 3, Name = "Algorithms", Parent = child };

            var ancestors = grandchild.GetAllAncestors();

            Assert.AreEqual(2, ancestors.Count);
            Assert.IsTrue(ancestors.Contains(child));
            Assert.IsTrue(ancestors.Contains(root));
        }

        [TestMethod]
        public void TestGetAllAncestorsForRootDomain()
        {
            var ancestors = this.domain.GetAllAncestors();
            Assert.AreEqual(0, ancestors.Count);
        }

        [TestMethod]
        public void TestGetAllAncestorsForDomainWithOneParent()
        {
            var parent = new Domain { DomainId = 2, Name = "Technology" };
            this.domain.Parent = parent;

            var ancestors = this.domain.GetAllAncestors();

            Assert.AreEqual(1, ancestors.Count);
            Assert.IsTrue(ancestors.Contains(parent));
        }

        [TestMethod]
        public void TestDomainWithBooks()
        {
            var book1 = new Book { BookId = 1, Title = "Book One" };
            var book2 = new Book { BookId = 2, Title = "Book Two" };

            this.domain.Books.Add(book1);
            this.domain.Books.Add(book2);

            Assert.AreEqual(2, this.domain.Books.Count);
        }

        [TestMethod]
        public void TestParentDomainWithChildDomain()
        {
            var childDomain = new Domain
            {
                DomainId = 3,
                Name = "Subfield",
                Parent = this.domain
            };

            Assert.IsNotNull(childDomain.Parent);
            Assert.AreEqual(this.domain.DomainId, childDomain.Parent.DomainId);
        }

    }
}
