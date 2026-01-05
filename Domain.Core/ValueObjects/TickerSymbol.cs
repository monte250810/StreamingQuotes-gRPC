namespace Domain.Core.ValueObjects;
public sealed record TickerSymbol
{
    public string Value { get; }
    private TickerSymbol(string value) => Value = value;
    public static TickerSymbol Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Ticker symbol cannot be empty", nameof(value));

        if (value.Length > 10)
            throw new ArgumentException("Ticker symbol cannot exceed 10 characters", nameof(value));

        return new TickerSymbol(value.ToUpperInvariant().Trim());
    }
    public static implicit operator string(TickerSymbol ticker) => ticker.Value;
    public override string ToString() => Value;
}
