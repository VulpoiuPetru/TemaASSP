using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="EditionValidator"/>
    /// </summary>
    [TestClass]
    public class EditionValidatorTests
    {
        private EditionValidator _validator;

        /// <summary>
        /// Initializes EditionValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new EditionValidator();
        }

        /// <summary>
        /// Validation should fail when Publisher is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenPublisherIsEmpty_ShouldHaveError()
        {
            var edition = new Edition { Publisher = "" };

            var result = _validator.TestValidate(edition);

            result.ShouldHaveValidationErrorFor(e => e.Publisher);
        }

        /// <summary>
        /// Validation should fail when NumberOfPages is too small
        /// </summary>
        [TestMethod]
        public void Validate_WhenNumberOfPagesIsInvalid_ShouldHaveError()
        {
            var edition = new Edition { NumberOfPages = 1 };

            var result = _validator.TestValidate(edition);

            result.ShouldHaveValidationErrorFor(e => e.NumberOfPages);
        }

        /// <summary>
        /// Validation should fail when Book is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenBookIsNull_ShouldHaveError()
        {
            var edition = new Edition { Book = null };

            var result = _validator.TestValidate(edition);

            result.ShouldHaveValidationErrorFor(e => e.Book);
        }
    }
}
