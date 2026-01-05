using Domain.Core.Entities;
using Domain.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core.Interface.Infrastructure.Persistence
{
    public interface ICryptoAssetProvider
    {
        Task<CryptoAsset?> GetByIdAsync(SymbolId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CryptoAsset>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CryptoAsset>> GetByIdsAsync(IEnumerable<SymbolId> ids, CancellationToken cancellationToken = default);
        IAsyncEnumerable<CryptoAsset> StreamAllAsync(CancellationToken cancellationToken = default);
        IAsyncEnumerable<CryptoAsset> StreamByIdsAsync(IEnumerable<SymbolId> ids, int intervalMs, CancellationToken cancellationToken = default);
        Task AddAsync(CryptoAsset asset, CancellationToken cancellationToken = default);
        Task UpdateAsync(CryptoAsset asset, CancellationToken cancellationToken = default);
    }
}
