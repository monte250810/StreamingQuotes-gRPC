namespace Domain.Core.Exceptions;

public sealed class RateLimitExceededException : DomainException
{
    public TimeSpan RetryAfter { get; }

    public RateLimitExceededException(TimeSpan retryAfter)
        : base("RATE_LIMIT_EXCEEDED", $"API rate limit exceeded. Retry after {retryAfter.TotalSeconds:F0} seconds.")
    {
        RetryAfter = retryAfter;
    }
}