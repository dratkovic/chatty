using Chatty.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Application.Common.Repositories;

public interface IChattyDbContext
{
    DbSet<T> Set<T>() where T : EntityBase;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
    int SaveChanges();
}
