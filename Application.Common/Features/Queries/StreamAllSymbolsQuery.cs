using Application.Common.DTOs;
using Application.Common.Mappings;
using Application.Common.Messaging.Interfaces;
using Domain.Core.Interface.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
