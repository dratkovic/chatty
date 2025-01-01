namespace Sensei.Core.Domain.Models;

public interface IFactory<out TEntity>
    where TEntity : AggregateRoot
{
    TEntity Build();
}