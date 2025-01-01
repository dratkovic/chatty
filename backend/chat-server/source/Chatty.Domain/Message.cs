using Chatty.Core.Domain.Models;
using ErrorOr;

namespace Chatty.Domain;

public class Message : EntityBase
{
    public Guid SenderId { get; private set; }
    public string Content { get; private set; }
    
    public DateTime TimeStampUtc { get; private set; }
    public Guid? GroupId { get; private set; } // Null for 1:1 chats
    public Guid? RecipientId { get; private set; } // Null for group chats
    
    public MessageStatus Status { get; private set; } = MessageStatus.Sent;
    
    private Message(Guid senderId, string content, Guid? groupId, Guid? recipientId) : base(null)
    {
        SenderId = senderId;
        Content = content;
        GroupId = groupId;
        RecipientId = recipientId;
        TimeStampUtc = DateTime.UtcNow;
    }
    
    internal static ErrorOr<Message> Create(Guid senderId, string content, Guid? groupId, Guid? recipientId)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Error.Validation("Message content cannot be empty");
        }
        if(recipientId == null && groupId == null)
        {
            return Error.Validation("Message must have a recipient or a group");
        }
        if(groupId != null && recipientId != null)
        {
            return Error.Validation("Message cannot have both a recipient and a group");
        }
        return new Message(senderId, content, groupId, recipientId);
    }
    
    internal ErrorOr<Success> ChangeStatus(MessageStatus newStatus)
    {
        if (Status >= newStatus)
        {
            return Error.Validation("Message status cannot be regressed");
        }
        Status = newStatus;
        return new Success();
    }
}

public enum MessageStatus
{
    Sent,
    Delivered,
    Read
}