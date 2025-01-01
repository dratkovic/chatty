using Chatty.Core.Domain;

namespace Chatty.Domain.Events;

public sealed record UserJoinedGroupEvent(User User, Group Group) : IDomainEvent
{
}