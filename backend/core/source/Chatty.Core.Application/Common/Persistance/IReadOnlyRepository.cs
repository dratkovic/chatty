using System.Linq.Expressions;
using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Persistance;

public interface IReadOnlyRepository<T> where T: EntityBase
{
    IPaginatedRepository<T> Pagination { get; }
    Task<T> GetByIdAsync(Guid id);
    Task<List<T>> ListAllAsync();
    Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> Query();
}