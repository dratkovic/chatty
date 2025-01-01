namespace Chatty.Core.Domain.Models;

public interface IFactory<out TEntity>
    where TEntity : AggregateRoot
{
    TEntity Build();
}