namespace Chatty.Contracts.Requests;

public sealed record SendMessageRequest(
    string Content, 
    Guid? ReceiverId, 
    Guid? GroupId);