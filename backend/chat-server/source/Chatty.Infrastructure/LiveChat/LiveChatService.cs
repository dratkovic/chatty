using System.Collections.Concurrent;
using Chatty.Application.Services;
using Chatty.Contracts.Responses;
using Microsoft.AspNetCore.SignalR;

namespace Chatty.Infrastructure.LiveChat;

public class LiveChatService: ILiveChatService
{
    private readonly IHubContext<LiveChatHub, ILiveChatClient> _hubContext;

    public LiveChatService(IHubContext<LiveChatHub, ILiveChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task DeliverMessageToUser(MessageResponse message)
    {
        return _hubContext.Clients.User(message.RecipientId.ToString() ?? string.Empty).ReceiveMessage(message);
    }
}