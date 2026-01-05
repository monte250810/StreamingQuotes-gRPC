using FluentValidation;

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
