using Application.Common.DTOs;
using Domain.Core.Entities;

namespace Application.Common.Mappings
{
    public static class CryptoAssetMappings
    {
        public static CryptoAssetDto ToDto(this CryptoAsset asset) => new()
        {
            Id = asset.Id.Value,
            Symbol = asset.Ticker.Value,
            Name = asset.Name,
            CurrentPrice = asset.CurrentPrice.Amount,
            MarketCap = asset.MarketCap.Amount,
            Volume24H = asset.Volume24H.Amount,
            PriceChange24H = asset.PriceChange24H.Value,
            High24H = asset.Range24H.High.Amount,
            Low24H = asset.Range24H.Low.Amount,
            MarketCapRank = asset.MarketCapRankValue,
            ImageUrl = asset.ImageUrl,
            Trend = asset.GetTrend().ToString(),
            LastUpdatedAt = asset.LastUpdatedAt
        };

        public static PriceUpdateDto ToPriceUpdate(this CryptoAsset asset) => new()
        {
            SymbolId = asset.Id.Value,
            Ticker = asset.Ticker.Value,
            Price = asset.CurrentPrice.Amount,
            PriceChange24H = asset.PriceChange24H.Value,
            High24H = asset.Range24H.High.Amount,
            Low24H = asset.Range24H.Low.Amount,
            Timestamp = asset.LastUpdatedAt
        };

        public static IEnumerable<CryptoAssetDto> ToDtos(this IEnumerable<CryptoAsset> assets) =>
            assets.Select(ToDto);
    }
}
