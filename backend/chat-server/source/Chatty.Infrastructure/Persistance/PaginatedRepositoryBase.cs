using Chatty.Application.Common.Pagination;
using Chatty.Application.Common.Repositories;
using Chatty.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Infrastructure.Persistance;

public class PaginatedRepository<T> : IPaginatedRepository<T> where T : EntityBase
{
    public async Task<PaginationResult<T>> GetPaginatedAsync(PaginationQuery pagination, IQueryable<T> query, CancellationToken token)
    {
        var totalRecords = await query.CountAsync(token);

        var skip = (pagination.Page - 1) * pagination.PageSize;

        query = query.Skip(skip).Take(pagination.PageSize);
        var data = await query.ToListAsync(token);

        return new PaginationResult<T>(data, totalRecords, pagination.Page, pagination.PageSize);
    }
}
