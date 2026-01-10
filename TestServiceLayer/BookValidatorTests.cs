using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using System.Collections.Generic;
using FluentValidation.TestHelper;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="BookValidator"/>
    /// </summary>
    [TestClass]
    public class BookValidatorTests
    {
        private BookValidator _validator;

        /// <summary>
        /// Initializes BookValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new BookValidator();
        }

        /// <summary>
        /// Validation should fail when Title is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenTitleIsEmpty_ShouldHaveError()
        {
            var book = new Book { Title = "" };

            var result = _validator.TestValidate(book);

            result.ShouldHaveValidationErrorFor(b => b.Title);
        }

        /// <summary>
        /// Validation should fail when Authors collection is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenNoAuthors_ShouldHaveError()
        {
            var book = new Book
            {
                Authors = new List<Author>()
            };

            var result = _validator.TestValidate(book);

            result.ShouldHaveValidationErrorFor(b => b.Authors);
        }

        /// <summary>
        /// Validation should fail when Domains collection is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenNoDomains_ShouldHaveError()
        {
            var book = new Book
            {
                Domains = new List<Domain>()
            };

            var result = _validator.TestValidate(book);

            result.ShouldHaveValidationErrorFor(b => b.Domains);
        }

        /// <summary>
        /// Validation should fail when Edition is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenEditionIsNull_ShouldHaveError()
        {
            var book = new Book { Edition = null };

            var result = _validator.TestValidate(book);

            result.ShouldHaveValidationErrorFor(b => b.Edition);
        }
    }
}
