using Chatty.Core.Application.Common.Pagination;
using Chatty.Core.Domain.Models;

namespace Chatty.Core.Application.Common.Persistance;

public interface IPaginatedRepository<T> where T : EntityBase
{
    Task<PaginationResult<T>> GetPaginatedAsync(PaginationQuery pagination, IQueryable<T> query, CancellationToken token);
}
