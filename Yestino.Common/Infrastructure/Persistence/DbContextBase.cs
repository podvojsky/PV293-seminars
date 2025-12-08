using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Domain;

namespace Yestino.Common.Infrastructure.Persistence;

public abstract class DbContextBase(DbContextOptions options, IMessageBus bus) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAggregateRootVersions();
        await PublishDomainEventsAsync();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAggregateRootVersions()
    {
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregateRoot in aggregateRoots) aggregateRoot.Version += 1;
    }

    private async Task PublishDomainEventsAsync()
    {
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        aggregateRoots.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents) await bus.PublishAsync(domainEvent);
    }
}
