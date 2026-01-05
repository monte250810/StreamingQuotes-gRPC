using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CoinGegko.Options
{
    public sealed class CoinGeckoOptions
    {
        public const string SectionName = "CoinGecko";

        [Required]
        [Url]
        public string BaseUrl { get; init; } = "https://api.coingecko.com/api/v3";

        public string? ApiKey { get; init; }

        [Range(1, 300)]
        public int TimeoutSeconds { get; init; } = 30;

        [Range(1000, 60000)]
        public int MinIntervalMs { get; init; } = 10000;

        [Range(1, 250)]
        public int MaxSymbolsPerRequest { get; init; } = 50;

        [Range(1, 5)]
        public int MaxRetries { get; init; } = 3;

        public string DefaultCurrency { get; init; } = "usd";
    }
}
