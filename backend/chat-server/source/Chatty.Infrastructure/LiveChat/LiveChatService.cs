using Chatty.Application.Services;
using Chatty.Contracts.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chatty.Infrastructure.LiveChat;

public class LiveChatService : ILiveChatService
{
    private readonly IHubContext<LiveChatHub, ILiveChatClient> _hubContext;
    private readonly ILogger<LiveChatService> _logger;

    public LiveChatService(IHubContext<LiveChatHub, ILiveChatClient> hubContext, ILogger<LiveChatService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public Task DeliverMessageToUser(MessageResponse message)
    {
        try
        {
            return _hubContext.Clients.User(message.RecipientId.ToString() ?? string.Empty).ReceiveMessage(message);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to deliver message to user {UserId}", message.RecipientId);
        }
        
        return Task.CompletedTask;
    }
}