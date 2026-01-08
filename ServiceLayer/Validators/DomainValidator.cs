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
    /// Validator for Domain entity
    /// </summary>
    public class DomainValidator : AbstractValidator<Domain>
    {
        public DomainValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(5, 50);

            RuleFor(x => x)
                .Must(d => d.Parent == null || d.Parent.DomainId != d.DomainId)
                .WithMessage("Domain cannot be its own parent");
        }
    }
}
