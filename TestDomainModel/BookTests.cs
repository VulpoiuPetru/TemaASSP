using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{

    /// <summary>
    /// Tests for the Book domain model class
    /// </summary>
    [TestClass]
    public class BookTests
    {
        private Book book;
        private Domain domain;
        private Author author;
        private Edition edition;

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

            this.author = new Author
            {
                AuthorId = 1,
                FirstName = "Isaac",
                LastName = "Asimov",
                Age = 72
            };

            this.edition = new Edition
            {
                EditionId = 1,
                Publisher = "Publisher Name",
                NumberOfPages = 300,
                YearOfPublishing = 2020,
                Type = "Hardcover"
            };

            this.book = new Book
            {
                BookId = 1,
                Title = "Foundation",
                NumberOfTotalBooks = 10,
                NumberOfReadingRoomBooks = 2,
                NumberOfAvailableBooks = 8,
                Edition = this.edition
            };

            this.book.Domains.Add(this.domain);
            this.book.Authors.Add(this.author);
        }

        [TestMethod]
        public void TestCorrectBook()
        {
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNullTitle()
        {
            this.book.Title = null;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestTitleTooShort()
        {
            this.book.Title = "1984";
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestTitleMinimumLength()
        {
            this.book.Title = "Robot";
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestTitleTooLong()
        {
            this.book.Title = new string('a', 51);
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestTitleMaximumLength()
        {
            this.book.Title = new string('a', 50);
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestEmptyTitle()
        {
            this.book.Title = string.Empty;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNullEdition()
        {
            this.book.Edition = null;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfTotalBooksNegative()
        {
            this.book.NumberOfTotalBooks = -1;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfTotalBooksZero()
        {
            this.book.NumberOfTotalBooks = 0;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfReadingRoomBooksNegative()
        {
            this.book.NumberOfReadingRoomBooks = -1;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfReadingRoomBooksZero()
        {
            this.book.NumberOfReadingRoomBooks = 0;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfAvailableBooksNegative()
        {
            this.book.NumberOfAvailableBooks = -1;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfAvailableBooksZero()
        {
            this.book.NumberOfAvailableBooks = 0;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }

        [TestMethod]
        public void TestDomainsCollectionInitialized()
        {
            var newBook = new Book();
            Assert.IsNotNull(newBook.Domains);
            Assert.AreEqual(0, newBook.Domains.Count);
        }

        [TestMethod]
        public void TestAuthorsCollectionInitialized()
        {
            var newBook = new Book();
            Assert.IsNotNull(newBook.Authors);
            Assert.AreEqual(0, newBook.Authors.Count);
        }

        [TestMethod]
        public void TestBookWithMultipleDomains()
        {
            var domain2 = new Domain { DomainId = 2, Name = "Mathematics" };
            this.book.Domains.Add(domain2);

            Assert.AreEqual(2, this.book.Domains.Count);
        }

        [TestMethod]
        public void TestBookWithMultipleAuthors()
        {
            var author2 = new Author
            {
                AuthorId = 2,
                FirstName = "Arthur",
                LastName = "Clarke",
                Age = 90
            };
            this.book.Authors.Add(author2);

            Assert.AreEqual(2, this.book.Authors.Count);
        }

        [TestMethod]
        public void TestBookIdCanBeSet()
        {
            this.book.BookId = 999;
            Assert.AreEqual(999, this.book.BookId);
        }

        [TestMethod]
        public void TestNumberOfTotalBooksLargeValue()
        {
            this.book.NumberOfTotalBooks = int.MaxValue;
            var context = new ValidationContext(this.book, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.book, context, results, true));
        }
    }
}
