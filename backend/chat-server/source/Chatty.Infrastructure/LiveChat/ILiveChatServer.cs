using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;

namespace Chatty.Infrastructure.LiveChat;

public interface ILiveChatServer
{
    Task<MessageStatusResponse> SendMessage(SendMessageRequest message);
}