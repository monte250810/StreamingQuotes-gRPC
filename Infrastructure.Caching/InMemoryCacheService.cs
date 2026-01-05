using Domain.Core.Interface;
using Infrastructure.Caching.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching
{

    internal sealed class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly CachingOptions _options;
        private readonly ILogger<InMemoryCacheService> _logger;

        public InMemoryCacheService(
            IMemoryCache cache,
            IOptions<CachingOptions> options,
            ILogger<InMemoryCacheService> logger)
        {
            _cache = cache;
            _options = options.Value;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return Task.FromResult<T?>(default);

            var value = _cache.Get<T>(key);

            if (value is not null)
                _logger.LogTrace("Cache hit: {Key}", key);

            return Task.FromResult(value);
        }

        public Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return Task.CompletedTask;

            var expirationTime = expiration ?? TimeSpan.FromSeconds(_options.DefaultExpirationSeconds);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expirationTime)
                .SetSlidingExpiration(TimeSpan.FromSeconds(expirationTime.TotalSeconds / 2));

            _cache.Set(key, value, cacheOptions);
            _logger.LogTrace("Cache set: {Key} (expires in {Expiration})", key, expirationTime);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _cache.Remove(key);
            _logger.LogTrace("Cache removed: {Key}", key);
            return Task.CompletedTask;
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return await factory();

            var cached = await GetAsync<T>(key, cancellationToken);

            if (cached is not null)
                return cached;

            var value = await factory();
            await SetAsync(key, value, expiration, cancellationToken);

            return value;
        }
    }
}
