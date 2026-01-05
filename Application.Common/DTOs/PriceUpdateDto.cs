namespace Application.Common.DTOs
{
    public sealed record PriceUpdateDto
    {
        public required string SymbolId { get; init; }
        public required string Ticker { get; init; }
        public required decimal Price { get; init; }
        public required decimal PriceChange24H { get; init; }
        public required decimal High24H { get; init; }
        public required decimal Low24H { get; init; }
        public required DateTime Timestamp { get; init; }
    }
}
