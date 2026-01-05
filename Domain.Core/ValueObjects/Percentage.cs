namespace Domain.Core.ValueObjects;

public sealed record Percentage
{
    public decimal? Value { get; }
    private Percentage(decimal? value) => Value = value;
    public static Percentage Create(decimal? value) => new(value);
    public static Percentage Zero => new(0);
    public bool IsPositive => Value > 0;
    public bool IsNegative => Value < 0;
    public override string ToString() => $"{Value:+0.00;-0.00;0.00}%";
}
