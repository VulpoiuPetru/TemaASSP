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
    /// Validator for Extension entity
    /// </summary>
    public class ExtensionValidator : AbstractValidator<Extension>
    {
        public ExtensionValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0);

            RuleFor(x => x.ReaderId)
                .GreaterThan(0);

            RuleFor(x => x.ExtensionDays)
                .InclusiveBetween(1, 90);

            RuleFor(x => x.RequestDate)
                .LessThanOrEqualTo(DateTime.Now);

            RuleFor(x => x.BorrowedBooks)
                .NotNull();
        }
    }
}
