using Chatty.Domain.Common;

namespace Chatty.Domain.Events;

public sealed record  MessagePostedEvent(Message Message) : IDomainEvent
{
    
}