using System.Linq.Expressions;
using Chatty.Core.Domain.Models;

namespace Chatty.Core.Application.Common.Persistance;

public interface IReadOnlyRepository<T> where T: EntityBase
{
    IPaginatedRepository<T> Pagination { get; }
    Task<T> GetByIdAsync(Guid id);
    Task<List<T>> ListAllAsync();
    Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
}