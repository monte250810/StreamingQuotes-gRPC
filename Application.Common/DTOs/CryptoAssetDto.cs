namespace Application.Common.DTOs
{
    public sealed record CryptoAssetDto
    {
        public required string Id { get; init; }
        public required string Symbol { get; init; }
        public required string Name { get; init; }
        public required decimal? CurrentPrice { get; init; }
        public required decimal? MarketCap { get; init; }
        public required decimal? Volume24H { get; init; }
        public required decimal? PriceChange24H { get; init; }
        public required decimal? High24H { get; init; }
        public required decimal? Low24H { get; init; }
        public int? MarketCapRank { get; init; }
        public string? ImageUrl { get; init; }
        public required string Trend { get; init; }
        public required DateTime LastUpdatedAt { get; init; }
    }
}
