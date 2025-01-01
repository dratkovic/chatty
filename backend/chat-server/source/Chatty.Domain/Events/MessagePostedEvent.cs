using Chatty.Core.Domain;

namespace Chatty.Domain.Events;

public sealed record  MessagePostedEvent(Message Message) : IDomainEvent
{
    
}