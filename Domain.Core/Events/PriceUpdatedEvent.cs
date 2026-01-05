namespace Domain.Core.Events
{
    public sealed record PriceUpdatedEvent(
        string SymbolId,
        string Ticker,
        decimal OldPrice,
        decimal NewPrice) : DomainEvent;
}
