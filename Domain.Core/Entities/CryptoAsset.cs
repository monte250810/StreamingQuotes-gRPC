using Domain.Core.Abstractions;
using Domain.Core.Enums;
using Domain.Core.Events;
using Domain.Core.Exceptions;
using Domain.Core.ValueObjects;

namespace Domain.Core.Entities;
public sealed class CryptoAsset : AggregateRoot<SymbolId>
{
    public TickerSymbol Ticker { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public Money CurrentPrice { get; private set; } = null!;
    public Money MarketCap { get; private set; } = null!;
    public Money Volume24H { get; private set; } = null!;
    public Percentage PriceChange24H { get; private set; } = null!;
    public PriceRange Range24H { get; private set; } = null!;
    public int? MarketCapRankValue { get; private set; }
    public CryptoCategory Category { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public string? ImageUrl { get; private set; }

    private CryptoAsset() { } // EF Core

    private CryptoAsset(
        SymbolId id,
        TickerSymbol ticker,
        string name,
        Money currentPrice,
        Money marketCap,
        Money volume24H,
        Percentage priceChange24H,
        PriceRange range24H,
        int? marketCapRank,
        CryptoCategory category,
        string? imageUrl)
    {
        Id = id;
        Ticker = ticker;
        Name = name;
        CurrentPrice = currentPrice;
        MarketCap = marketCap;
        Volume24H = volume24H;
        PriceChange24H = priceChange24H;
        Range24H = range24H;
        MarketCapRankValue = marketCapRank;
        Category = category;
        ImageUrl = imageUrl;
        LastUpdatedAt = DateTime.UtcNow;
    }

    public static CryptoAsset Create(
        string id,
        string ticker,
        string name,
        decimal currentPrice,
        decimal marketCap,
        decimal volume24H,
        decimal priceChange24H,
        decimal high24H,
        decimal low24H,
        int? marketCapRank = null,
        CryptoCategory category = CryptoCategory.Unknown,
        string? imageUrl = null)
    {
        if (currentPrice < 0)
            throw new InvalidPriceException(currentPrice);

        var asset = new CryptoAsset(
            SymbolId.Create(id),
            TickerSymbol.Create(ticker),
            name,
            Money.Create(currentPrice),
            Money.Create(marketCap),
            Money.Create(volume24H),
            Percentage.Create(priceChange24H),
            PriceRange.Create(high24H, low24H),
            marketCapRank,
            category,
            imageUrl);

        asset.RaiseDomainEvent(new CryptoAssetCreatedEvent(asset.Id.Value, asset.Ticker.Value, asset.CurrentPrice.Amount));

        return asset;
    }

    public void UpdatePrice(decimal newPrice, decimal high24H, decimal low24H, decimal priceChange24H)
    {
        if (newPrice < 0)
            throw new InvalidPriceException(newPrice);

        var oldPrice = CurrentPrice.Amount;
        CurrentPrice = Money.Create(newPrice);
        Range24H = PriceRange.Create(high24H, low24H);
        PriceChange24H = Percentage.Create(priceChange24H);
        LastUpdatedAt = DateTime.UtcNow;

        IncrementVersion();

        if (Math.Abs(newPrice - oldPrice) / oldPrice > 0.05m) // 5% change
        {
            RaiseDomainEvent(new SignificantPriceChangeEvent(Id.Value, oldPrice, newPrice));
        }

        RaiseDomainEvent(new PriceUpdatedEvent(Id.Value, Ticker.Value, oldPrice, newPrice));
    }

    public void UpdateMarketData(decimal marketCap, decimal volume24H, int? rank)
    {
        MarketCap = Money.Create(marketCap);
        Volume24H = Money.Create(volume24H);
        MarketCapRankValue = rank;
        IncrementVersion();
    }

    public PriceTrend GetTrend()
    {
        return PriceChange24H.Value switch
        {
            > 10 => PriceTrend.Bullish,
            > 5 => PriceTrend.Bullish,
            < -10 => PriceTrend.Bearish,
            < -5 => PriceTrend.Bearish,
            _ when Range24H.SpreadPercentage > 15 => PriceTrend.HighlyVolatile,
            _ => PriceTrend.Stable
        };
    }

    public MarketCapRank GetMarketCapRank()
    {
        return MarketCapRankValue switch
        {
            null => Enums.MarketCapRank.Unknown,
            <= 10 => Enums.MarketCapRank.Top10,
            <= 50 => Enums.MarketCapRank.Top50,
            <= 100 => Enums.MarketCapRank.Top100,
            <= 500 => Enums.MarketCapRank.Top500,
            <= 1000 => Enums.MarketCapRank.Top1000,
            _ => Enums.MarketCapRank.Other
        };
    }
}