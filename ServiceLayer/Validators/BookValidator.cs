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
    /// Validator for Book entity
    /// </summary>
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.Authors)
                .NotNull()
                .Must(a => a.Any())
                .WithMessage("Book must have at least one author");

            RuleFor(x => x.Domains)
                .NotNull()
                .Must(d => d.Any())
                .WithMessage("Book must belong to at least one domain");

            RuleFor(x => x.NumberOfTotalBooks)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.NumberOfReadingRoomBooks)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.NumberOfTotalBooks);

            RuleFor(x => x.NumberOfAvailableBooks)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.NumberOfTotalBooks);

            RuleFor(x => x.Edition)
                .NotNull();
        }
    }
}
