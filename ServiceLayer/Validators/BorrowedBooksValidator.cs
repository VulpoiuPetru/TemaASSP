using DomainModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators
{
    /// <summary>
    /// Validator for BorrowedBooks entity
    /// </summary>
    public class BorrowedBooksValidator : AbstractValidator<BorrowedBooks>
    {
        /// <summary>
        /// Initializes a new instance of the BorrowedBooksValidator class
        /// </summary>
        public BorrowedBooksValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0);

            RuleFor(x => x.ReaderId)
                .GreaterThan(0);

            RuleFor(x => x.BorrowStartDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.BorrowEndDate)
                .NotEmpty()
                .GreaterThan(x => x.BorrowStartDate);

            RuleFor(x => x.Book)
                .NotNull();

            RuleFor(x => x.Reader)
                .NotNull();

        }

    }



}
