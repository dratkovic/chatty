using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain.Models;
using Chatty.Core.Infrastructure.Common.Persistence;
using Chatty.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Sensei.Infrastructure.Persistance;

public class SenseiDbContext: AppDbContext, IAppDbContext
{
    public SenseiDbContext(DbContextOptions options,
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
        base.OnModelCreating(modelBuilder);

    }

    public new DbSet<T> Set<T>() where T : EntityBase
    {
        return base.Set<T>();
    }
}