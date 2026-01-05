using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Domain.Core.Interface.Infrastructure.Persistence;
using Domain.Core.ValueObjects;
using System.Runtime.CompilerServices;

namespace Application.Common.Features.Queries
{
    public sealed record StreamPricesQuery(
        IReadOnlyList<string> SymbolIds,
        int IntervalMs = 15000) : IStreamQuery<PriceUpdateDto>;

    internal sealed class StreamPricesQueryHandler : IStreamQueryHandler<StreamPricesQuery, PriceUpdateDto>
    {
        private readonly ICryptoAssetProvider _repository;
        public StreamPricesQueryHandler(ICryptoAssetProvider repository) => _repository = repository;

        public async IAsyncEnumerable<PriceUpdateDto> Handle(
            StreamPricesQuery request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var symbolIds = request.SymbolIds.Select(SymbolId.Create).ToList();

            await foreach (var asset in _repository.StreamByIdsAsync(symbolIds, request.IntervalMs, cancellationToken))
            {
                yield return asset.ToPriceUpdate();
            }
        }
    }
}
