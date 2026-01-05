namespace Domain.Core.Abstractions;

public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
{
    public DateTime CreatedAt { get; protected init; }
    public DateTime? ModifiedAt { get; protected set; }
    public int Version { get; protected set; }

    protected AggregateRoot()
    {
        CreatedAt = DateTime.UtcNow;
        Version = 1;
    }

    protected void IncrementVersion()
    {
        Version++;
        ModifiedAt = DateTime.UtcNow;
    }
}