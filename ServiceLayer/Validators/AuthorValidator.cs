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
    /// Validator for Author entity
    /// </summary>
    public class AuthorValidator : AbstractValidator<Author>
    {
        public AuthorValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x.Age)
                .InclusiveBetween(10, 80);
        }
    }
}
