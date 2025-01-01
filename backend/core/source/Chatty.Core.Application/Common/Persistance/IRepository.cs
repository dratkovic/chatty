using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Persistance;

public interface IRepository<T>: IReadOnlyRepository<T> where T: EntityBase
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}