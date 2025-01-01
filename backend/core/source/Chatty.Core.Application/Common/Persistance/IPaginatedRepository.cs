using Sensei.Core.Application.Common.Pagination;
using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Persistance;

public interface IPaginatedRepository<T> where T : EntityBase
{
    Task<PaginationResult<T>> GetPaginatedAsync(PaginationQuery pagination, IQueryable<T> query, CancellationToken token);
}
