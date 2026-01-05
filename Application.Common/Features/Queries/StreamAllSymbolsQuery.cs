using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Domain.Core.Interface.Infrastructure.Persistence;
using System.Runtime.CompilerServices;

namespace Application.Common.Features.Queries
{
    public sealed record StreamAllSymbolsQuery : IStreamQuery<CryptoAssetDto>;
    internal sealed class StreamAllSymbolsQueryHandler : IStreamQueryHandler<StreamAllSymbolsQuery, CryptoAssetDto>
    {
        private readonly ICryptoAssetProvider _repository;

        public StreamAllSymbolsQueryHandler(ICryptoAssetProvider repository)
        {
            _repository = repository;
        }

        public async IAsyncEnumerable<CryptoAssetDto> Handle(
            StreamAllSymbolsQuery request,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var asset in _repository.StreamAllAsync(cancellationToken))
            {
                yield return asset.ToDto();
            }
        }
    }
}
