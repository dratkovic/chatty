using System.Reflection;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Domain;
using Chatty.Core.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Core.Infrastructure.Common.Persistence;

public class AppDbContext : DbContext
{
    private readonly IPublisher _publisher;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AppDbContext(DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IAuthenticatedUserProvider authenticatedUserProvider
        ) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        _publisher = publisher;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task CommitChangesAsync(CancellationToken token)
    {
        // get hold of all the domain events
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        // store them in the http context for later if user is waiting online
        if (IsUserWaitingOnline())
        {
            AddDomainEventsToOfflineProcessingQueue(domainEvents);
        }
        else
        {
            await PublishDomainEvents(_publisher, domainEvents);
        }

        await SaveChangesAsync(token);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        MarkAuditable();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        MarkAuditable();
        return base.SaveChanges();
    }

    private void MarkAuditable()
    {
        foreach (var entry in ChangeTracker.Entries<AuditEntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _authenticatedUserProvider.GetCurrentUser().Id.ToString();
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _authenticatedUserProvider.GetCurrentUser().Id.ToString();
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _authenticatedUserProvider.GetCurrentUser().Id.ToString();
                    break;
            }
        }
    }

    private static async Task PublishDomainEvents(IPublisher _publisher, List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }

    private bool IsUserWaitingOnline() => _httpContextAccessor.HttpContext is not null;

    private void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
    {
        // fetch queue from http context or create a new queue if it doesn't exist
        var domainEventsQueue = _httpContextAccessor.HttpContext!.Items
            .TryGetValue("DomainEventsQueue", out var value) && value is Queue<IDomainEvent> existingDomainEvents
                ? existingDomainEvents
                : new Queue<IDomainEvent>();

        // add the domain events to the end of the queue
        domainEvents.ForEach(domainEventsQueue.Enqueue);

        // store the queue in the http context
        _httpContextAccessor.HttpContext!.Items["DomainEventsQueue"] = domainEventsQueue;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.HasDefaultSchema("dbo");

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configBuilder)
    {
        configBuilder.DefaultTypeMapping<decimal>(b => b.HasPrecision(18, 4));
        configBuilder.Properties<decimal>().HavePrecision(18, 4);
    }
}