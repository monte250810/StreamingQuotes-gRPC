using System.Text.Json.Serialization;

namespace Infrastructure.CoinGegko.Models
{
    public sealed record CoinGeckoMarketData
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("symbol")]
        public string Symbol { get; init; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; init; } = string.Empty;

        [JsonPropertyName("image")]
        public string? Image { get; init; }

        [JsonPropertyName("current_price")]
        public decimal CurrentPrice { get; init; }

        [JsonPropertyName("market_cap")]
        public decimal MarketCap { get; init; }

        [JsonPropertyName("market_cap_rank")]
        public int? MarketCapRank { get; init; }

        [JsonPropertyName("fully_diluted_valuation")]
        public decimal? FullyDilutedValuation { get; init; }

        [JsonPropertyName("total_volume")]
        public decimal TotalVolume { get; init; }

        [JsonPropertyName("high_24h")]
        public decimal High24H { get; init; }

        [JsonPropertyName("low_24h")]
        public decimal Low24H { get; init; }

        [JsonPropertyName("price_change_24h")]
        public decimal PriceChange24H { get; init; }

        [JsonPropertyName("price_change_percentage_24h")]
        public decimal PriceChangePercentage24H { get; init; }

        [JsonPropertyName("market_cap_change_24h")]
        public decimal MarketCapChange24H { get; init; }

        [JsonPropertyName("market_cap_change_percentage_24h")]
        public decimal MarketCapChangePercentage24H { get; init; }

        [JsonPropertyName("circulating_supply")]
        public decimal CirculatingSupply { get; init; }

        [JsonPropertyName("total_supply")]
        public decimal? TotalSupply { get; init; }

        [JsonPropertyName("max_supply")]
        public decimal? MaxSupply { get; init; }

        [JsonPropertyName("ath")]
        public decimal AllTimeHigh { get; init; }

        [JsonPropertyName("ath_change_percentage")]
        public decimal AllTimeHighChangePercentage { get; init; }

        [JsonPropertyName("ath_date")]
        public DateTime AllTimeHighDate { get; init; }

        [JsonPropertyName("atl")]
        public decimal AllTimeLow { get; init; }

        [JsonPropertyName("atl_change_percentage")]
        public decimal AllTimeLowChangePercentage { get; init; }

        [JsonPropertyName("atl_date")]
        public DateTime AllTimeLowDate { get; init; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; init; }
    }
}
