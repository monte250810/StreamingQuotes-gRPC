using System.Text.Json.Serialization;

namespace Infrastructure.CoinGegko.Models
{
    public sealed record CoinGeckoPing
    {
        [JsonPropertyName("gecko_says")]
        public string GeckoSays { get; init; } = string.Empty;
    }
}
