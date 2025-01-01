using Chatty.Core.Domain;

namespace Chatty.Domain.Events;

public sealed record  GroupCreatedEvent(Group Group) : IDomainEvent
{
}