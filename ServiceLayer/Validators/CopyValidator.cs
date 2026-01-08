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
    /// Validator for Copy entity
    /// </summary>
    public class CopyValidator : AbstractValidator<Copy>
    {
        public CopyValidator()
        {
            RuleFor(x => x.Edition)
                .NotNull()
                .WithMessage("Copy must be associated with an edition");
        }
    }
}
