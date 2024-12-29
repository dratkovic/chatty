using Chatty.Domain.Common;

namespace Chatty.Domain.Events;

public sealed record UserJoinedGroupEvent(User User, Group Group) : IDomainEvent
{
}