using Domain.Core.Entities;
using Domain.Core.Enums;
using Infrastructure.CoinGegko.Models;

namespace Infrastructure.CoinGegko.ExternalServices.Mappings
{
    internal static class CoinGeckoMappings
    {
        public static CryptoAsset ToDomainEntity(this CoinGeckoMarketData data)
        {
            return CryptoAsset.Create(
                id: data.Id,
                ticker: data.Symbol,
                name: data.Name,
                currentPrice: data.CurrentPrice ?? 0,
                marketCap: data.MarketCap ?? 0,
                volume24H: data.TotalVolume ?? 0,
                priceChange24H: data.PriceChangePercentage24H ?? 0,
                high24H: data.High24H ?? 0,
                low24H: data.Low24H ?? 0,
                marketCapRank: data.MarketCapRank,
                category: CryptoCategory.Unknown,
                imageUrl: data.Image);
        }

        public static IEnumerable<CryptoAsset> ToDomainEntities(this IEnumerable<CoinGeckoMarketData> data)
            => data.Where(d => d.CurrentPrice.HasValue) // Filter out coins with no price data
                   .Select(ToDomainEntity);
    }
}
