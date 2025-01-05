using Chatty.Contracts.Responses;

namespace Chatty.Infrastructure.LiveChat;

public interface ILiveChatClient
{
    Task ReceiveMessage(MessageResponse message);
}