namespace Chatty.Contracts.Responses;

public record MessageResponse(
    Guid Id,
    Guid SenderId,
    string SenderDisplayName,
    string Content,
    Guid? RecipientId,
    Guid? GroupId,
    DateTime TimeStampUtc,
    string Status) : MessageStatusResponse(Id, RecipientId, GroupId, TimeStampUtc, Status);
