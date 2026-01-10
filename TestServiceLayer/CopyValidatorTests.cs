using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;


namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="CopyValidator"/>
    /// Verifies validation rules for the Copy entity
    /// </summary>
    [TestClass]
    public class CopyValidatorTests
    {
        private CopyValidator _validator;

        /// <summary>
        /// Initializes a new instance of <see cref="CopyValidator"/>
        /// before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new CopyValidator();
        }

        /// <summary>
        /// Validation should fail when Copy.Edition is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenEditionIsNull_ShouldHaveValidationError()
        {
            var copy = new Copy
            {
                Edition = null
            };

            var result = _validator.TestValidate(copy);

            result.ShouldHaveValidationErrorFor(c => c.Edition);
        }

        /// <summary>
        /// Validation should succeed when Copy.Edition is not null
        /// </summary>
        [TestMethod]
        public void Validate_WhenEditionIsNotNull_ShouldNotHaveValidationError()
        {
            var copy = new Copy
            {
                Edition = new Edition()
            };

            var result = _validator.TestValidate(copy);

            result.ShouldNotHaveValidationErrorFor(c => c.Edition);
        }
    }
}
