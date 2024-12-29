using Chatty.Domain.Common;

namespace Chatty.Domain.Events;

public sealed record  GroupCreatedEvent(Group Group) : IDomainEvent
{
}