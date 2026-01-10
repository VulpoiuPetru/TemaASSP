using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{

    /// <summary>
    /// Tests for the Edition domain model class
    /// </summary>
    [TestClass]
    public class EditionTests
    {
        private Edition edition;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.edition = new Edition
            {
                EditionId = 1,
                Publisher = "Test Publisher",
                NumberOfPages = 300,
                YearOfPublishing = 2020,
                Type = "Hardcover",
                Book = new Book { BookId = 1, Title = "Test Book" }
            };
        }

        [TestMethod]
        public void TestCorrectEdition()
        {
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestNullPublisher()
        {
            this.edition.Publisher = null;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestPublisherTooShort()
        {
            this.edition.Publisher = "Pub";
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestPublisherMinimumLength()
        {
            this.edition.Publisher = "Books";
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestPublisherTooLong()
        {
            this.edition.Publisher = new string('a', 51);
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfPagesZero()
        {
            this.edition.NumberOfPages = 0;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfPagesNegative()
        {
            this.edition.NumberOfPages = -10;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfPagesPositive()
        {
            this.edition.NumberOfPages = 500;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestYearOfPublishingTooEarly()
        {
            this.edition.YearOfPublishing = 1399;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestYearOfPublishingMinimumBoundary()
        {
            this.edition.YearOfPublishing = 1400;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestYearOfPublishingCurrentYear()
        {
            this.edition.YearOfPublishing = DateTime.Now.Year;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestYearOfPublishingFutureYear()
        {
            this.edition.YearOfPublishing = DateTime.Now.Year + 1;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }


        [TestMethod]
        public void TestNullType()
        {
            this.edition.Type = null;
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestTypeTooShort()
        {
            this.edition.Type = "HC";
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestTypeMinimumLength()
        {
            this.edition.Type = "EBOOK";
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.edition, context, results, true));
        }

        [TestMethod]
        public void TestTypeTooLong()
        {
            this.edition.Type = new string('a', 51);
            var context = new ValidationContext(this.edition, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.edition, context, results, true));
        }


        [TestMethod]
        public void TestCopiesCollectionInitialized()
        {
            var newEdition = new Edition();
            Assert.IsNotNull(newEdition.Copies);
            Assert.AreEqual(0, newEdition.Copies.Count);
        }

        [TestMethod]
        public void TestEditionIdCanBeSet()
        {
            this.edition.EditionId = 999;
            Assert.AreEqual(999, this.edition.EditionId);
        }

        [TestMethod]
        public void TestEditionWithMultipleCopies()
        {
            var copy1 = new Copy { Id = 1, Edition = this.edition };
            var copy2 = new Copy { Id = 2, Edition = this.edition };

            this.edition.Copies.Add(copy1);
            this.edition.Copies.Add(copy2);

            Assert.AreEqual(2, this.edition.Copies.Count);
        }
    }
}
