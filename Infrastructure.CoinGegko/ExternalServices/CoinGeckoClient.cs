using Domain.Core.Exceptions;
using Infrastructure.CoinGegko.ExternalServices.Interfaces;
using Infrastructure.CoinGegko.Models;
using Infrastructure.CoinGegko.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace Infrastructure.CoinGegko.ExternalServices
{
    internal sealed class CoinGeckoClient : ICoinGeckoClient
    {
        private readonly HttpClient _httpClient;
        private readonly CoinGeckoOptions _options;
        private readonly ILogger<CoinGeckoClient> _logger;

        public CoinGeckoClient(
            HttpClient httpClient,
            IOptions<CoinGeckoOptions> options,
            ILogger<CoinGeckoClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyList<CoinGeckoMarketData>> GetMarketsAsync(
            int perPage = 50,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            var url = $"coins/markets?vs_currency={_options.DefaultCurrency}&order=market_cap_desc&per_page={perPage}&page={page}&sparkline=false&price_change_percentage=24h";

            _logger.LogDebug("Fetching markets: {Url}", url);

            return await ExecuteWithRetryAsync<List<CoinGeckoMarketData>>(url, cancellationToken) ?? [];
        }

        public async Task<IReadOnlyList<CoinGeckoMarketData>> GetMarketsByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            var idList = ids.ToList();
            if (idList.Count == 0)
                return [];

            if (idList.Count > _options.MaxSymbolsPerRequest)
            {
                _logger.LogWarning(
                    "Requested {Count} symbols, but max is {Max}. Truncating.",
                    idList.Count,
                    _options.MaxSymbolsPerRequest);
                idList = idList.Take(_options.MaxSymbolsPerRequest).ToList();
            }

            var idsParam = string.Join(",", idList);
            var url = $"coins/markets?vs_currency={_options.DefaultCurrency}&ids={idsParam}&order=market_cap_desc&sparkline=false&price_change_percentage=24h";

            _logger.LogDebug("Fetching markets by IDs: {Ids}", idsParam);

            return await ExecuteWithRetryAsync<List<CoinGeckoMarketData>>(url, cancellationToken) ?? [];
        }

        public async Task<CoinGeckoPing> PingAsync(CancellationToken cancellationToken = default)
        {
            return await ExecuteWithRetryAsync<CoinGeckoPing>("ping", cancellationToken)
                   ?? new CoinGeckoPing { GeckoSays = "Error" };
        }

        private async Task<T?> ExecuteWithRetryAsync<T>(string url, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                    _logger.LogWarning("Rate limited by CoinGecko. Retry after: {RetryAfter}", retryAfter);
                    throw new RateLimitExceededException(retryAfter);
                }

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
            }
            catch (RateLimitExceededException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while calling CoinGecko API: {Url}", url);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while calling CoinGecko API: {Url}", url);
                throw;
            }
        }
    }
}
