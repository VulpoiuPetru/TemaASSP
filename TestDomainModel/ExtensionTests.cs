using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{
    /// <summary>
    /// Tests for the Extension domain model class
    /// </summary>
    [TestClass]
    public class ExtensionTests
    {
        private Extension extension;
        private BorrowedBooks borrowedBooks;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.borrowedBooks = new BorrowedBooks
            {
                BookId = 1,
                ReaderId = 1,
                BorrowStartDate = DateTime.Now,
                BorrowEndDate = DateTime.Now.AddDays(14)
            };

            this.extension = new Extension
            {
                ExtensionId = 1,
                BookId = 1,
                ReaderId = 1,
                RequestDate = DateTime.Now,
                ExtensionDays = 7,
                BorrowedBooks = this.borrowedBooks
            };
        }

        [TestMethod]
        public void TestCorrectExtension()
        {
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestBookIdZero()
        {
            this.extension.BookId = 0;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestBookIdNegative()
        {
            this.extension.BookId = -1;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestBookIdPositive()
        {
            this.extension.BookId = 100;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestReaderIdZero()
        {
            this.extension.ReaderId = 0;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestReaderIdNegative()
        {
            this.extension.ReaderId = -1;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestReaderIdPositive()
        {
            this.extension.ReaderId = 200;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysZero()
        {
            this.extension.ExtensionDays = 0;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysNegative()
        {
            this.extension.ExtensionDays = -5;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysAtMinimumBoundary()
        {
            this.extension.ExtensionDays = 1;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysAtMaximumBoundary()
        {
            this.extension.ExtensionDays = 90;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysAboveMaximum()
        {
            this.extension.ExtensionDays = 91;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestExtensionDaysMidRange()
        {
            this.extension.ExtensionDays = 30;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestNullBorrowedBooks()
        {
            this.extension.BorrowedBooks = null;
            var context = new ValidationContext(this.extension, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.extension, context, results, true));
        }

        [TestMethod]
        public void TestRequestDateCanBeSet()
        {
            var testDate = new DateTime(2024, 1, 15);
            this.extension.RequestDate = testDate;
            Assert.AreEqual(testDate, this.extension.RequestDate);
        }

        [TestMethod]
        public void TestExtensionIdCanBeSet()
        {
            this.extension.ExtensionId = 999;
            Assert.AreEqual(999, this.extension.ExtensionId);
        }

        [TestMethod]
        public void TestExtensionWithValidBorrowedBooks()
        {
            Assert.IsNotNull(this.extension.BorrowedBooks);
            Assert.AreEqual(1, this.extension.BorrowedBooks.BookId);
        }
    }
}
