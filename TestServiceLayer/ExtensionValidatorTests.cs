using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="ExtensionValidator"/>
    /// </summary>
    [TestClass]
    public class ExtensionValidatorTests
    {
        private ExtensionValidator _validator;

        /// <summary>
        /// Initializes ExtensionValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new ExtensionValidator();
        }

        /// <summary>
        /// Validation should fail when BookId is less than or equal to zero
        /// </summary>
        [TestMethod]
        public void Validate_WhenBookIdIsInvalid_ShouldHaveError()
        {
            var extension = new Extension { BookId = 0 };

            var result = _validator.TestValidate(extension);

            result.ShouldHaveValidationErrorFor(e => e.BookId);
        }

        /// <summary>
        /// Validation should fail when ExtensionDays is outside allowed range
        /// </summary>
        [TestMethod]
        public void Validate_WhenExtensionDaysIsInvalid_ShouldHaveError()
        {
            var extension = new Extension { ExtensionDays = 100 };

            var result = _validator.TestValidate(extension);

            result.ShouldHaveValidationErrorFor(e => e.ExtensionDays);
        }

        /// <summary>
        /// Validation should fail when RequestDate is in the future
        /// </summary>
        [TestMethod]
        public void Validate_WhenRequestDateIsInFuture_ShouldHaveError()
        {
            var extension = new Extension { RequestDate = DateTime.Now.AddDays(1) };

            var result = _validator.TestValidate(extension);

            result.ShouldHaveValidationErrorFor(e => e.RequestDate);
        }
    }
}
