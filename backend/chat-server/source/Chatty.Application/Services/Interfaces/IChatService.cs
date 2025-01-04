using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;

namespace Chatty.Application.Services;

public interface IChatService
{
    Task<bool> IsUserConnectedToNode(Guid userId);
    
    Task<MessageStatusResponse> SendMessageAsync(SendMessageRequest message);
    
    Task ReceiveMessageAsync(MessageResponse message);
}