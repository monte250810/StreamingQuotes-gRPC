using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Caching.Options
{
    internal class CachingOptions
    {
        public const string SectionName = "Caching";

        public bool Enabled { get; init; } = true;

        [Range(1, 3600)]
        public int DefaultExpirationSeconds { get; init; } = 60;

        [Range(1, 3600)]
        public int PriceDataExpirationSeconds { get; init; } = 30;

        [Range(1, 86400)]
        public int SymbolListExpirationSeconds { get; init; } = 300;
    }
}
