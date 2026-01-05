namespace Domain.Core.Events
{
    public sealed record CryptoAssetCreatedEvent(
        string SymbolId,
        string Ticker,
        decimal? InitialPrice) : DomainEvent;
}
