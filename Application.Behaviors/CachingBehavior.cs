using Domain.Core.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
namespace Application.Behaviors
{
    public interface ICachedQuery
    {
        string CacheKey { get; }
        TimeSpan? CacheDuration { get; }
    }

    public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

        public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (request is not ICachedQuery cachedQuery)
                return await next();

            var cachedResult = await _cacheService.GetAsync<TResponse>(cachedQuery.CacheKey, cancellationToken);

            if (cachedResult is not null)
            {
                _logger.LogDebug("Cache hit for {CacheKey}", cachedQuery.CacheKey);
                return cachedResult;
            }

            _logger.LogDebug("Cache miss for {CacheKey}", cachedQuery.CacheKey);

            var response = await next();

            await _cacheService.SetAsync(
                cachedQuery.CacheKey,
                response,
                cachedQuery.CacheDuration,
                cancellationToken);

            _logger.LogDebug("Cached response for {CacheKey}", cachedQuery.CacheKey);

            return response;
        }
    }
}