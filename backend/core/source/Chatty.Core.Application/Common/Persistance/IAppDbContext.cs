using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Persistance;

public interface IAppDbContext
{
    DbSet<T> Set<T>() where T : EntityBase;
    DatabaseFacade Database { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    Task CommitChangesAsync(CancellationToken token);
}
