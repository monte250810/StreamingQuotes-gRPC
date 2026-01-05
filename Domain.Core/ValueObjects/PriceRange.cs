namespace Domain.Core.ValueObjects;

public sealed record PriceRange
{
    public Money High { get; }
    public Money Low { get; }
    private PriceRange(Money high, Money low)
    {
        High = high;
        Low = low;
    }
    public static PriceRange Create(decimal? high, decimal? low, string currency = "USD")
    {
        if (high < low)
            throw new ArgumentException("High price cannot be less than low price");

        return new PriceRange(Money.Create(high, currency), Money.Create(low, currency));
    }

    public decimal? Spread => High.Amount - Low.Amount;
    public decimal? SpreadPercentage => Low.Amount > 0 ? (Spread / Low.Amount) * 100 : 0;

    public override string ToString() => $"H: {High} / L: {Low}";
}
