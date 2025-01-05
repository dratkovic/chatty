using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain.Models;
using Chatty.Core.Infrastructure.Common.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Infrastructure.Persistance;

public class ChattyDbContext: AppDbContext, IAppDbContext
{
    public ChattyDbContext(DbContextOptions options,
        IHttpContextAccessor httpContextAccessor,
        IPublisher publisher,
        IAuthenticatedUserProvider authenticatedUserProvider) : base(options,
        httpContextAccessor,
        publisher,
        authenticatedUserProvider)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IInfrastructureMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public new DbSet<T> Set<T>() where T : EntityBase
    {
        return base.Set<T>();
    }
}