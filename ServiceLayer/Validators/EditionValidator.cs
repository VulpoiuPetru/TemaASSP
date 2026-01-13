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
    /// Validator for Edition entity
    /// </summary>
    public class EditionValidator : AbstractValidator<Edition>
    {
        public EditionValidator()
        {
            RuleFor(x => x.Publisher)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.NumberOfPages)
                .GreaterThanOrEqualTo(3);

            RuleFor(x => x.YearOfPublishing)
                .InclusiveBetween(1400, 2100);

            RuleFor(x => x.Type)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.Book)
                .NotNull();
        }
    }
}
