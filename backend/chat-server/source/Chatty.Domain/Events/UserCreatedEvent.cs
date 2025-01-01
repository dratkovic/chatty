using Chatty.Core.Domain;

namespace Chatty.Domain.Events;

public sealed record UserCreatedEvent(User User) : IDomainEvent{}
