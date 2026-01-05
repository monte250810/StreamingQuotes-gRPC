using Application.Common.Features.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Features.Queries.Validators
{
    public sealed class GetSymbolByIdQueryValidator : AbstractValidator<GetSymbolByIdQuery>
    {
        public GetSymbolByIdQueryValidator()
        {
            RuleFor(x => x.SymbolId)
                .NotEmpty()
                .WithMessage("Symbol ID is required")
                .MaximumLength(100)
                .WithMessage("Symbol ID cannot exceed 100 characters");
        }
    }
}
