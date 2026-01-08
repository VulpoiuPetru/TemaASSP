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
    /// Validator for Reader entity
    /// </summary>
    public class ReaderValidator : AbstractValidator<Reader>
    {
        public ReaderValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.Age)
                .InclusiveBetween(10, 80);

            RuleFor(x => x)
                .Must(r => r.HasValidContact())
                .WithMessage("Either email or phone number must be provided");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.NumberOfExtensions)
                .GreaterThanOrEqualTo(0);
        }
    }
}
