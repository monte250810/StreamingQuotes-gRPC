using Application.Behaviors.Results;
using Application.Common.DTOs;
using Application.Common.Mappings;
using Application.Common.Messaging.Interfaces;
using Domain.Core.Interface.Infrastructure.Persistence;

namespace Application.Common.Features.Queries
{
    public sealed record GetAllSymbolsQuery(int? Limit = null) : IQuery<IReadOnlyList<CryptoAssetDto>>
    {
        public string CacheKey => $"crypto:symbols:all:{Limit ?? 0}";
        public TimeSpan? CacheDuration => TimeSpan.FromMinutes(1);
    }

    internal sealed class GetAllSymbolsQueryHandler : IQueryHandler<GetAllSymbolsQuery, IReadOnlyList<CryptoAssetDto>>
    {
        private readonly ICryptoAssetProvider _repository;

        public GetAllSymbolsQueryHandler(ICryptoAssetProvider repository)
        {
            _repository = repository;
        }

        public async Task<Result<IReadOnlyList<CryptoAssetDto>>> Handle(
            GetAllSymbolsQuery request,
            CancellationToken cancellationToken)
        {
            var assets = await _repository.GetAllAsync(cancellationToken);

            var result = request.Limit.HasValue
                ? assets.Take(request.Limit.Value).ToDtos().ToList()
                : assets.ToDtos().ToList();

            return result;
        }
    }
}
