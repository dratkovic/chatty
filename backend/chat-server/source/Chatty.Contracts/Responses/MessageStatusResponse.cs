namespace Chatty.Contracts.Responses;

public record MessageStatusResponse(
    Guid Id,
    Guid? RecipientId,
    Guid? GroupId,
    DateTime TimeStampUtc,
    string Status
    );