namespace Chatty.Domain.Common;

public abstract class AggregateRoot: AuditEntityBase
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(Guid? id) : base(id)
    {}
    
    public IList<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();

        _domainEvents.Clear();

        return copy;
    }
    
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (domainEvent == null || _domainEvents.Contains(domainEvent))
        {
            return;
        }
        _domainEvents.Add(domainEvent);
    }
}