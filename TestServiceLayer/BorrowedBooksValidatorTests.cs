using DomainModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLayer.Validators;
using System;
using FluentValidation.TestHelper;

namespace TestServiceLayer
{
    /// <summary>
    /// Unit tests for <see cref="BorrowedBooksValidator"/>
    /// </summary>
    [TestClass]
    public class BorrowedBooksValidatorTests
    {
        private BorrowedBooksValidator _validator;

        /// <summary>
        /// Initializes BorrowedBooksValidator before each test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _validator = new BorrowedBooksValidator();
        }

        /// <summary>
        /// Validation should fail when BorrowEndDate is before BorrowStartDate
        /// </summary>
        [TestMethod]
        public void Validate_WhenBorrowEndDateIsInvalid_ShouldHaveError()
        {
            var borrowed = new BorrowedBooks
            {
                BorrowStartDate = DateTime.Now,
                BorrowEndDate = DateTime.Now.AddDays(-1)
            };

            var result = _validator.TestValidate(borrowed);

            result.ShouldHaveValidationErrorFor(b => b.BorrowEndDate);
        }

        /// <summary>
        /// Validation should fail when Book is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenBookIsNull_ShouldHaveError()
        {
            var borrowed = new BorrowedBooks { Book = null };

            var result = _validator.TestValidate(borrowed);

            result.ShouldHaveValidationErrorFor(b => b.Book);
        }

        /// <summary>
        /// Validation should fail when Reader is null
        /// </summary>
        [TestMethod]
        public void Validate_WhenReaderIsNull_ShouldHaveError()
        {
            var borrowed = new BorrowedBooks { Reader = null };

            var result = _validator.TestValidate(borrowed);

            result.ShouldHaveValidationErrorFor(b => b.Reader);
        }
    }
}
