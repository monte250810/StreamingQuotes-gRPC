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
}
