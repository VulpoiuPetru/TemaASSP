using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DomainModel;

namespace TestDomainModel
{

    /// <summary>
    /// Tests for the Author domain model class
    /// </summary>
    [TestClass]
    public class AuthorTests
    {
        private Author author;


        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.author = new Author
            {
                FirstName = "George",
                LastName = "Orwell",
                Age = 46
            };
        }

        [TestMethod]
        public void TestCorrectAuthor()
        {
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestNullFirstName()
        {
            this.author.FirstName = null;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameTooShort()
        {
            this.author.FirstName = "Jon";
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameMinimumLength()
        {
            this.author.FirstName = "James";
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameTooLong()
        {
            this.author.FirstName = new string('a', 51);
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameMaximumLength()
        {
            this.author.FirstName = new string('a', 50);
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestEmptyFirstName()
        {
            this.author.FirstName = string.Empty;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestNullLastName()
        {
            this.author.LastName = null;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestLastNameTooShort()
        {
            this.author.LastName = "Lee";
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestLastNameMinimumLength()
        {
            this.author.LastName = "Smith";
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestLastNameTooLong()
        {
            this.author.LastName = new string('b', 51);
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestLastNameMaximumLength()
        {
            this.author.LastName = new string('b', 50);
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestEmptyLastName()
        {
            this.author.LastName = string.Empty;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeBelowMinimum()
        {
            this.author.Age = 9;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeAboveMaximum()
        {
            this.author.Age = 81;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeAtMinimumBoundary()
        {
            this.author.Age = 10;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeAtMaximumBoundary()
        {
            this.author.Age = 80;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeZero()
        {
            this.author.Age = 0;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestAgeNegative()
        {
            this.author.Age = -5;
            var context = new ValidationContext(this.author, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.author, context, results, true));
        }

        [TestMethod]
        public void TestBooksCollectionInitialized()
        {
            var newAuthor = new Author();
            Assert.IsNotNull(newAuthor.Books);
            Assert.AreEqual(0, newAuthor.Books.Count);
        }

        [TestMethod]
        public void TestBooksCollectionNotNull()
        {
            Assert.IsNotNull(this.author.Books);
        }

        [TestMethod]
        public void TestAuthorIdCanBeSet()
        {
            this.author.AuthorId = 100;
            Assert.AreEqual(100, this.author.AuthorId);
        }

        [TestMethod]
        public void TestValidAuthorWithMultipleBooks()
        {
            var book1 = new Book { BookId = 1, Title = "Book One" };
            var book2 = new Book { BookId = 2, Title = "Book Two" };
            this.author.Books.Add(book1);
            this.author.Books.Add(book2);

            Assert.AreEqual(2, this.author.Books.Count);
        }
    }
}
