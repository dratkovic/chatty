using Chatty.Core.Domain;

namespace Chatty.Domain.Events;

public sealed record  UserRemovedFromGroupEvent(User Admin, User User, Group Group) : IDomainEvent 
{
}