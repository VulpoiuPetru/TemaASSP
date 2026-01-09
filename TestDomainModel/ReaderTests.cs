using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{
    /// <summary>
    /// Tests for the Reader domain model class
    /// </summary>
    [TestClass]
    public class ReaderTests
    {
        private Reader reader;

        /// <summary>
        /// Initializes test data before each test method
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.reader = new Reader
            {
                ReaderId = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                Age = 25,
                Email = "alice@example.com",
                PhoneNumber = "1234567890",
                IsEmployee = false,
                NumberOfExtensions = 0
            };
        }

        [TestMethod]
        public void TestCorrectReader()
        {
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestNullFirstName()
        {
            this.reader.FirstName = null;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameTooShort()
        {
            this.reader.FirstName = "Ann";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameMinimumLength()
        {
            this.reader.FirstName = "James";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestFirstNameTooLong()
        {
            this.reader.FirstName = new string('a', 51);
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestLastNameTooShort()
        {
            this.reader.LastName = "Doe";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestLastNameMinimumLength()
        {
            this.reader.LastName = "Smith";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestLastNameTooLong()
        {
            this.reader.LastName = new string('b', 51);
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestAgeBelowMinimum()
        {
            this.reader.Age = 9;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestAgeAboveMaximum()
        {
            this.reader.Age = 81;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestAgeAtMinimumBoundary()
        {
            this.reader.Age = 10;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestAgeAtMaximumBoundary()
        {
            this.reader.Age = 80;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestInvalidEmail()
        {
            this.reader.Email = "invalid-email";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestValidEmail()
        {
            this.reader.Email = "user@example.com";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestInvalidPhoneNumber()
        {
            this.reader.PhoneNumber = "abc123";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestValidPhoneNumber()
        {
            this.reader.PhoneNumber = "+1234567890";
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestHasValidContactWithEmail()
        {
            this.reader.Email = "test@example.com";
            this.reader.PhoneNumber = null;
            Assert.IsTrue(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestHasValidContactWithPhoneNumber()
        {
            this.reader.Email = null;
            this.reader.PhoneNumber = "1234567890";
            Assert.IsTrue(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestHasValidContactWithBoth()
        {
            this.reader.Email = "test@example.com";
            this.reader.PhoneNumber = "1234567890";
            Assert.IsTrue(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestHasNoValidContact()
        {
            this.reader.Email = null;
            this.reader.PhoneNumber = null;
            Assert.IsFalse(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestHasNoValidContactWithEmptyStrings()
        {
            this.reader.Email = string.Empty;
            this.reader.PhoneNumber = string.Empty;
            Assert.IsFalse(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestHasNoValidContactWithWhitespace()
        {
            this.reader.Email = "   ";
            this.reader.PhoneNumber = "   ";
            Assert.IsFalse(this.reader.HasValidContact());
        }

        [TestMethod]
        public void TestNumberOfExtensionsNegative()
        {
            this.reader.NumberOfExtensions = -1;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfExtensionsZero()
        {
            this.reader.NumberOfExtensions = 0;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestNumberOfExtensionsPositive()
        {
            this.reader.NumberOfExtensions = 5;
            var context = new ValidationContext(this.reader, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.reader, context, results, true));
        }

        [TestMethod]
        public void TestIsEmployeeFalse()
        {
            this.reader.IsEmployee = false;
            Assert.IsFalse(this.reader.IsEmployee);
        }

        [TestMethod]
        public void TestIsEmployeeTrue()
        {
            this.reader.IsEmployee = true;
            Assert.IsTrue(this.reader.IsEmployee);
        }

        [TestMethod]
        public void TestExtensionsCollectionInitialized()
        {
            var newReader = new Reader();
            Assert.IsNotNull(newReader.Extensions);
            Assert.AreEqual(0, newReader.Extensions.Count);
        }

        [TestMethod]
        public void TestReaderIdCanBeSet()
        {
            this.reader.ReaderId = 100;
            Assert.AreEqual(100, this.reader.ReaderId);
        }
    }
}
