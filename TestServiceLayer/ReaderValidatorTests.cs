using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;


namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="ReaderValidator"/>
    /// </summary>
    [TestClass]
    public class ReaderValidatorTests
    {
        private ReaderValidator _validator;

        /// <summary>
        /// Initializes ReaderValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new ReaderValidator();
        }

        /// <summary>
        /// Validation should fail when FirstName is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenFirstNameIsEmpty_ShouldHaveError()
        {
            var reader = new Reader { FirstName = "" };

            var result = _validator.TestValidate(reader);

            result.ShouldHaveValidationErrorFor(r => r.FirstName);
        }

        /// <summary>
        /// Validation should fail when no contact information is provided
        /// </summary>
        [TestMethod]
        public void Validate_WhenNoContactProvided_ShouldHaveError()
        {
            var reader = new Reader
            {
                Email = null,
                PhoneNumber = null
            };

            var result = _validator.TestValidate(reader);

            result.ShouldHaveValidationErrorFor(r => r);
        }

        /// <summary>
        /// Validation should fail when Email is invalid
        /// </summary>
        [TestMethod]
        public void Validate_WhenEmailIsInvalid_ShouldHaveError()
        {
            var reader = new Reader
            {
                Email = "invalid-email"
            };

            var result = _validator.TestValidate(reader);

            result.ShouldHaveValidationErrorFor(r => r.Email);
        }

        /// <summary>
        /// Validation should fail when NumberOfExtensions is negative
        /// </summary>
        [TestMethod]
        public void Validate_WhenNumberOfExtensionsIsNegative_ShouldHaveError()
        {
            var reader = new Reader
            {
                NumberOfExtensions = -1
            };

            var result = _validator.TestValidate(reader);

            result.ShouldHaveValidationErrorFor(r => r.NumberOfExtensions);
        }
    }
}
