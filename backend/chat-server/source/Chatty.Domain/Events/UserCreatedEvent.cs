using Chatty.Domain.Common;

namespace Chatty.Domain.Events;

public sealed record UserCreatedEvent(User User) : IDomainEvent{}
