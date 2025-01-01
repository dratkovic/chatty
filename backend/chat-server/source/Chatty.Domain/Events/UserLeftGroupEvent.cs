using Chatty.Domain.Common;

namespace Chatty.Domain.Events;

public sealed record UserLeftGroupEvent(User User, Group Group) : IDomainEvent
{
}