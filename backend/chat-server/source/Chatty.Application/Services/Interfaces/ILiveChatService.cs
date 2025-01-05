using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;

namespace Chatty.Application.Services;

public interface ILiveChatService
{
    Task DeliverMessageToUser(MessageResponse message);
}