namespace Domain.Core.Events
{
    public sealed record SignificantPriceChangeEvent(
        string SymbolId,
        decimal? OldPrice,
        decimal? NewPrice) : DomainEvent
    {
        public decimal? ChangePercentage => OldPrice > 0 ? ((NewPrice - OldPrice) / OldPrice) * 100 : 0;
    }
}
