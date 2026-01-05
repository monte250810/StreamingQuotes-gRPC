using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.CoinGegko.Models
{
    public sealed record CoinGeckoPing
    {
        [JsonPropertyName("gecko_says")]
        public string GeckoSays { get; init; } = string.Empty;
    }
}
