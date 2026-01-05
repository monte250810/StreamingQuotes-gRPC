using Domain.Core.Interface.Infrastructure.Persistence;

namespace Infrastructure.Persistence.Dal
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }
    }
}
