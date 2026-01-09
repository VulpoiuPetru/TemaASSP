using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestDomainModel
{
    /// <summary>
    /// Tests for the Copy domain model class
    /// </summary>
    [TestClass]
    public class CopyTests
    {
        private Copy copy;
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
                NumberOfPages = 200,
                YearOfPublishing = 2020,
                Type = "Hardcover"
            };

            this.copy = new Copy
            {
                Id = 1,
                Edition = this.edition,
                IsReadingRoomOnly = false,
                IsAvailable = true
            };
        }

        [TestMethod]
        public void TestCorrectCopy()
        {
            var context = new ValidationContext(this.copy, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsTrue(Validator.TryValidateObject(this.copy, context, results, true));
        }

        [TestMethod]
        public void TestNullEdition()
        {
            this.copy.Edition = null;
            var context = new ValidationContext(this.copy, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Assert.IsFalse(Validator.TryValidateObject(this.copy, context, results, true));
        }

        [TestMethod]
        public void TestCanBeBorrowedWhenAvailableAndNotReadingRoom()
        {
            this.copy.IsAvailable = true;
            this.copy.IsReadingRoomOnly = false;
            Assert.IsTrue(this.copy.CanBeBorrowed);
        }

        [TestMethod]
        public void TestCannotBeBorrowedWhenReadingRoomOnly()
        {
            this.copy.IsAvailable = true;
            this.copy.IsReadingRoomOnly = true;
            Assert.IsFalse(this.copy.CanBeBorrowed);
        }

        [TestMethod]
        public void TestCannotBeBorrowedWhenNotAvailable()
        {
            this.copy.IsAvailable = false;
            this.copy.IsReadingRoomOnly = false;
            Assert.IsFalse(this.copy.CanBeBorrowed);
        }

        [TestMethod]
        public void TestCannotBeBorrowedWhenReadingRoomOnlyAndNotAvailable()
        {
            this.copy.IsAvailable = false;
            this.copy.IsReadingRoomOnly = true;
            Assert.IsFalse(this.copy.CanBeBorrowed);
        }

        [TestMethod]
        public void TestIsReadingRoomOnlyTrue()
        {
            this.copy.IsReadingRoomOnly = true;
            Assert.IsTrue(this.copy.IsReadingRoomOnly);
        }

        [TestMethod]
        public void TestIsReadingRoomOnlyFalse()
        {
            this.copy.IsReadingRoomOnly = false;
            Assert.IsFalse(this.copy.IsReadingRoomOnly);
        }

        [TestMethod]
        public void TestIsAvailableTrue()
        {
            this.copy.IsAvailable = true;
            Assert.IsTrue(this.copy.IsAvailable);
        }

        [TestMethod]
        public void TestIsAvailableFalse()
        {
            this.copy.IsAvailable = false;
            Assert.IsFalse(this.copy.IsAvailable);
        }

        [TestMethod]
        public void TestCopyIdCanBeSet()
        {
            this.copy.Id = 999;
            Assert.AreEqual(999, this.copy.Id);
        }

        [TestMethod]
        public void TestCopyWithValidEdition()
        {
            Assert.IsNotNull(this.copy.Edition);
            Assert.AreEqual(1, this.copy.Edition.EditionId);
        }

        [TestMethod]
        public void TestCopyWithDifferentEditionId()
        {
            var edition2 = new Edition
            {
                EditionId = 5,
                Publisher = "Another Publisher",
                NumberOfPages = 300,
                YearOfPublishing = 2022,
                Type = "Paperback"
            };
            this.copy.Edition = edition2;
            Assert.AreEqual(5, this.copy.Edition.EditionId);
        }

        [TestMethod]
        public void TestMultipleCopiesOfSameEdition()
        {
            var copy2 = new Copy
            {
                Id = 2,
                Edition = this.edition,
                IsReadingRoomOnly = true,
                IsAvailable = true
            };

            Assert.AreEqual(this.copy.Edition.EditionId, copy2.Edition.EditionId);
            Assert.AreNotEqual(this.copy.IsReadingRoomOnly, copy2.IsReadingRoomOnly);
        }

        [TestMethod]
        public void TestToggleAvailability()
        {
            this.copy.IsAvailable = true;
            Assert.IsTrue(this.copy.IsAvailable);

            this.copy.IsAvailable = false;
            Assert.IsFalse(this.copy.IsAvailable);
        }

        [TestMethod]
        public void TestToggleReadingRoomOnly()
        {
            this.copy.IsReadingRoomOnly = false;
            Assert.IsFalse(this.copy.IsReadingRoomOnly);

            this.copy.IsReadingRoomOnly = true;
            Assert.IsTrue(this.copy.IsReadingRoomOnly);
        }
    }
}
