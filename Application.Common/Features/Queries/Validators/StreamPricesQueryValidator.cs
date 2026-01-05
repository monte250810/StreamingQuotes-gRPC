using FluentValidation;

namespace Application.Common.Features.Queries.Validators
{
    public sealed class StreamPricesQueryValidator : AbstractValidator<StreamPricesQuery>
    {
        public StreamPricesQueryValidator()
        {
            RuleFor(x => x.IntervalMs)
                .GreaterThanOrEqualTo(10000)
                .WithMessage("Interval must be at least 10 seconds due to API rate limits");

            RuleFor(x => x.SymbolIds)
                .Must(ids => ids.Count <= 50)
                .WithMessage("Cannot stream more than 50 symbols at once");
        }
    }
}
