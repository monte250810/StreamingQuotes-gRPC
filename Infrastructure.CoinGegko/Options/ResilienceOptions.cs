using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CoinGegko.Options
{
    public sealed class ResilienceOptions
    {
        public const string SectionName = "Resilience";

        [Range(1, 10)]
        public int MaxRetries { get; init; } = 3;

        [Range(100, 30000)]
        public int BaseDelayMs { get; init; } = 1000;

        [Range(1, 60)]
        public int CircuitBreakerDurationSeconds { get; init; } = 30;

        [Range(1, 20)]
        public int CircuitBreakerThreshold { get; init; } = 5;

        [Range(1000, 60000)]
        public int TimeoutMs { get; init; } = 30000;
    }
}
