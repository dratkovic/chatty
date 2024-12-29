using Chatty.Application.Common.Pagination;
using Chatty.Domain.Common;

namespace Chatty.Application.Common.Interfaces.Repositories;

public interface IPaginatedRepository<T> where T : EntityBase
{
    Task<PaginationResult<T>> GetPaginatedAsync(PaginationQuery pagination, IQueryable<T> query, CancellationToken token);
}
