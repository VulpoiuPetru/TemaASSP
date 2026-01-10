using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="AuthorValidator"/>
    /// </summary>
    [TestClass]
    public class AuthorValidatorTests
    {
        private AuthorValidator _validator;

        /// <summary>
        /// Initializes AuthorValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new AuthorValidator();
        }

        /// <summary>
        /// Validation should fail when FirstName is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenFirstNameIsEmpty_ShouldHaveError()
        {
            var author = new Author { FirstName = "" };

            var result = _validator.TestValidate(author);

            result.ShouldHaveValidationErrorFor(a => a.FirstName);
        }

        /// <summary>
        /// Validation should fail when LastName is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenLastNameIsEmpty_ShouldHaveError()
        {
            var author = new Author { LastName = "" };

            var result = _validator.TestValidate(author);

            result.ShouldHaveValidationErrorFor(a => a.LastName);
        }

        /// <summary>
        /// Validation should fail when Age is outside allowed range
        /// </summary>
        [TestMethod]
        public void Validate_WhenAgeIsInvalid_ShouldHaveError()
        {
            var author = new Author { Age = 5 };

            var result = _validator.TestValidate(author);

            result.ShouldHaveValidationErrorFor(a => a.Age);
        }
    }
}
