using Application.Behaviors.Results;
using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Domain.Core.Interface.Infrastructure.Persistence;
using Domain.Core.ValueObjects;

namespace Application.Common.Features.Queries
{
    public sealed record GetSymbolByIdQuery(string SymbolId) : IQuery<CryptoAssetDto>;
    internal sealed class GetSymbolByIdQueryHandler : IQueryHandler<GetSymbolByIdQuery, CryptoAssetDto>
    {
        private readonly ICryptoAssetProvider _repository;

        public GetSymbolByIdQueryHandler(ICryptoAssetProvider repository)
        {
            _repository = repository;
        }

        public async Task<Result<CryptoAssetDto>> Handle(
            GetSymbolByIdQuery request,
            CancellationToken cancellationToken)
        {
            var symbolId = SymbolId.Create(request.SymbolId);
            var asset = await _repository.GetByIdAsync(symbolId, cancellationToken);

            if (asset is null)
                return Result.Failure<CryptoAssetDto>(Error.NotFound("CryptoAsset", request.SymbolId));

            return asset.ToDto();
        }
    }
}
