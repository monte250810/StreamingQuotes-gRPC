using Domain.Core.Entities;
using Domain.Core.Enums;
using Domain.Core.Interface.Infrastructure.Persistence;
using Domain.Core.ValueObjects;
using Infrastructure.CoinGegko.ExternalServices.Interfaces;
using Infrastructure.CoinGegko.Models;
using Infrastructure.CoinGegko.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Infrastructure.CoinGegko.ExternalServices;
internal sealed class CryptoAssetProvider : ICryptoAssetProvider
{
    private readonly ICoinGeckoClient _coinGeckoClient;
    private readonly CoinGeckoOptions _options;
    private readonly ILogger<CryptoAssetProvider> _logger;

    public CryptoAssetProvider(
        ICoinGeckoClient coinGeckoClient,
        IOptions<CoinGeckoOptions> options,
        ILogger<CryptoAssetProvider> logger)
    {
        _coinGeckoClient = coinGeckoClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CryptoAsset?> GetByIdAsync(SymbolId id, CancellationToken cancellationToken = default)
    {
        var marketData = await _coinGeckoClient.GetMarketsByIdsAsync([id.Value], cancellationToken);
        return marketData.FirstOrDefault()?.ToDomainEntity();
    }

    public async Task<IReadOnlyList<CryptoAsset>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var marketData = await _coinGeckoClient.GetMarketsAsync(cancellationToken: cancellationToken);
        return marketData.ToDomainEntities().ToList();
    }

    public async Task<IReadOnlyList<CryptoAsset>> GetByIdsAsync(
        IEnumerable<SymbolId> ids,
        CancellationToken cancellationToken = default)
    {
        var idStrings = ids.Select(id => id.Value).ToList();
        var marketData = await _coinGeckoClient.GetMarketsByIdsAsync(idStrings, cancellationToken);
        return marketData.ToDomainEntities().ToList();
    }

    public async IAsyncEnumerable<CryptoAsset> StreamAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var marketData = await _coinGeckoClient.GetMarketsAsync(cancellationToken: cancellationToken);

        foreach (var data in marketData)
        {
            yield return data.ToDomainEntity();
        }
    }

    public async IAsyncEnumerable<CryptoAsset> StreamByIdsAsync(
        IEnumerable<SymbolId> ids,
        int intervalMs,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var idStrings = ids.Select(id => id.Value).ToList();

        if (idStrings.Count == 0)
        {
            idStrings = ["bitcoin", "ethereum", "solana", "cardano", "ripple", "dogecoin", "polkadot", "avalanche-2"];
        }

        var effectiveInterval = Math.Max(intervalMs, _options.MinIntervalMs);
        _logger.LogInformation(
            "Starting price stream for {Count} symbols with {Interval}ms interval",
            idStrings.Count,
            effectiveInterval);

        while (!cancellationToken.IsCancellationRequested)
        {
            IReadOnlyList<CoinGeckoMarketData> marketData;

            try
            {
                marketData = await _coinGeckoClient.GetMarketsByIdsAsync(idStrings, cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Error fetching prices, will retry after interval");
                await Task.Delay(effectiveInterval, cancellationToken);
                continue;
            }

            foreach (var data in marketData)
            {
                yield return data.ToDomainEntity();
            }

            _logger.LogDebug("Streamed {Count} price updates", marketData.Count);
            await Task.Delay(effectiveInterval, cancellationToken);
        }
    }

    public Task AddAsync(CryptoAsset asset, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("AddAsync called for {Id} - no-op for external API", asset.Id);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(CryptoAsset asset, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("UpdateAsync called for {Id} - no-op for external API", asset.Id);
        return Task.CompletedTask;
    }
}
internal static class CoinGeckoMappings
{
    public static CryptoAsset ToDomainEntity(this CoinGeckoMarketData data)
    {
        return CryptoAsset.Create(
            id: data.Id,
            ticker: data.Symbol,
            name: data.Name,
            currentPrice: data.CurrentPrice,
            marketCap: data.MarketCap,
            volume24H: data.TotalVolume,
            priceChange24H: data.PriceChangePercentage24H,
            high24H: data.High24H,
            low24H: data.Low24H,
            marketCapRank: data.MarketCapRank,
            category: CryptoCategory.Unknown,
            imageUrl: data.Image);
    }

    public static IEnumerable<CryptoAsset> ToDomainEntities(this IEnumerable<CoinGeckoMarketData> data)
        => data.Select(ToDomainEntity);
}