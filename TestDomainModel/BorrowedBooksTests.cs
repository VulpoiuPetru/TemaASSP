using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{
    /// <summary>
    /// Tests for the BorrowedBooks domain model class
    /// </summary>
    [TestClass]
    public class BorrowedBooksTests
    {
        private BorrowedBooks borrowedBooks;
        private Book book;
        private Reader reader;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.book = new Book
            {
                BookId = 1,
                Title = "Test Book"
            };

            this.reader = new Reader
            {
                ReaderId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Age = 25,
                Email = "alice@test.com",
                IsEmployee = false
            };

            this.borrowedBooks = new BorrowedBooks
            {
                BookId = 1,
                ReaderId = 1,
                BorrowStartDate = DateTime.Now,
                BorrowEndDate = DateTime.Now.AddDays(14),
                Book = this.book,
                Reader = this.reader
            };
        }

        [TestMethod]
        public void TestCorrectBorrowedBooks()
        {
            var context = new ValidationContext(this.borrowedBooks, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.borrowedBooks, context, results, true));
        }

        [TestMethod]
        public void TestNullBook()
        {
            this.borrowedBooks.Book = null;
            var context = new ValidationContext(this.borrowedBooks, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.borrowedBooks, context, results, true));
        }

        [TestMethod]
        public void TestNullReader()
        {
            this.borrowedBooks.Reader = null;
            var context = new ValidationContext(this.borrowedBooks, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.borrowedBooks, context, results, true));
        }

        [TestMethod]
        public void TestBorrowStartDateCanBeSet()
        {
            var testDate = new DateTime(2024, 1, 15);
            this.borrowedBooks.BorrowStartDate = testDate;
            Assert.AreEqual(testDate, this.borrowedBooks.BorrowStartDate);
        }

        [TestMethod]
        public void TestBorrowEndDateCanBeSet()
        {
            var testDate = new DateTime(2024, 1, 29);
            this.borrowedBooks.BorrowEndDate = testDate;
            Assert.AreEqual(testDate, this.borrowedBooks.BorrowEndDate);
        }

        [TestMethod]
        public void TestBorrowEndDateExtendedCanBeSet()
        {
            var testDate = new DateTime(2024, 2, 5);
            this.borrowedBooks.BorrowEndDateExtended = testDate;
            Assert.AreEqual(testDate, this.borrowedBooks.BorrowEndDateExtended);
        }

        [TestMethod]
        public void TestBorrowEndDateAfterStartDate()
        {
            this.borrowedBooks.BorrowStartDate = new DateTime(2024, 1, 1);
            this.borrowedBooks.BorrowEndDate = new DateTime(2024, 1, 15);
            Assert.IsTrue(this.borrowedBooks.BorrowEndDate > this.borrowedBooks.BorrowStartDate);
        }

        [TestMethod]
        public void TestBookIdCanBeSet()
        {
            this.borrowedBooks.BookId = 100;
            Assert.AreEqual(100, this.borrowedBooks.BookId);
        }

        [TestMethod]
        public void TestReaderIdCanBeSet()
        {
            this.borrowedBooks.ReaderId = 200;
            Assert.AreEqual(200, this.borrowedBooks.ReaderId);
        }

        [TestMethod]
        public void TestBorrowEndDateExtendedAfterEndDate()
        {
            this.borrowedBooks.BorrowEndDate = new DateTime(2024, 1, 15);
            this.borrowedBooks.BorrowEndDateExtended = new DateTime(2024, 1, 29);
            Assert.IsTrue(this.borrowedBooks.BorrowEndDateExtended > this.borrowedBooks.BorrowEndDate);
        }

        [TestMethod]
        public void TestBorrowedBooksWithValidBook()
        {
            Assert.IsNotNull(this.borrowedBooks.Book);
            Assert.AreEqual(1, this.borrowedBooks.Book.BookId);
        }

        [TestMethod]
        public void TestBorrowedBooksWithValidReader()
        {
            Assert.IsNotNull(this.borrowedBooks.Reader);
            Assert.AreEqual(1, this.borrowedBooks.Reader.ReaderId);
        }

        [TestMethod]
        public void TestExtensionsCanBeSet()
        {
            var extensions = new List<Extension>
    {
        new Extension()
    };

            this.borrowedBooks.Extensions = extensions;

            Assert.IsNotNull(this.borrowedBooks.Extensions);
            Assert.AreEqual(1, this.borrowedBooks.Extensions.Count);
        }
    }
}
