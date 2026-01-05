using Infrastructure.CoinGegko.Models;

namespace Infrastructure.CoinGegko.ExternalServices.Interfaces
{
    public interface ICoinGeckoClient
    {
        Task<IReadOnlyList<CoinGeckoMarketData>> GetMarketsAsync(
            int perPage = 50,
            int page = 1,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CoinGeckoMarketData>> GetMarketsByIdsAsync(
            IEnumerable<string> ids,
            CancellationToken cancellationToken = default);

        Task<CoinGeckoPing> PingAsync(CancellationToken cancellationToken = default);
    }
}
