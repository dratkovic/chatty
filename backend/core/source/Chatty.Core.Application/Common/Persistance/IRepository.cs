using Chatty.Core.Domain.Models;

namespace Chatty.Core.Application.Common.Persistance;

public interface IRepository<T>: IReadOnlyRepository<T> where T: EntityBase
{
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}