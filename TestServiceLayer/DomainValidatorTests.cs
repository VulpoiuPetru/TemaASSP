using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;


namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="DomainValidator"/>
    /// Verifies validation rules for the Domain entity
    /// </summary>
    [TestClass]
    public class DomainValidatorTests
    {
        private DomainValidator _validator;

        /// <summary>
        /// Initializes the DomainValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new DomainValidator();
        }

        /// <summary>
        /// Validation should fail when Domain.Name is empty
        /// </summary>
        [TestMethod]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
        {
            var domain = new Domain { Name = "" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(d => d.Name);
        }

        /// <summary>
        /// Validation should fail when Domain.Name is shorter than 5 characters
        /// </summary>
        [TestMethod]
        public void Validate_WhenNameIsTooShort_ShouldHaveValidationError()
        {
            var domain = new Domain { Name = "Abc" };

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(d => d.Name);
        }

        /// <summary>
        /// Validation should succeed when Domain.Name is valid
        /// </summary>
        [TestMethod]
        public void Validate_WhenNameIsValid_ShouldNotHaveValidationError()
        {
            var domain = new Domain { Name = "Valid Name" };

            var result = _validator.TestValidate(domain);

            result.ShouldNotHaveValidationErrorFor(d => d.Name);
        }

        /// <summary>
        /// Validation should fail when Domain is its own parent
        /// </summary>
        [TestMethod]
        public void Validate_WhenDomainIsItsOwnParent_ShouldHaveValidationError()
        {
            var domain = new Domain { DomainId = 1 };
            domain.Parent = domain;

            var result = _validator.TestValidate(domain);

            result.ShouldHaveValidationErrorFor(d => d);
        }

        /// <summary>
        /// Validation should succeed when Domain.Parent is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenParentIsNull_ShouldNotHaveValidationError()
        {
            var domain = new Domain
            {
                DomainId = 1,
                Parent = null
            };

            var result = _validator.TestValidate(domain);

            result.ShouldNotHaveValidationErrorFor(d => d);
        }
    }
}
