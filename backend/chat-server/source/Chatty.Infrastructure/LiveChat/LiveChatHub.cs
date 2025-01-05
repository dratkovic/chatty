using System.Security.Claims;
using Chatty.Application.Features.Message.Commands;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Chatty.Infrastructure.LiveChat;

public class LiveChatHub: Hub<ILiveChatClient>, ILiveChatServer
{
    private readonly ISender _mediator;

    public LiveChatHub(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<MessageStatusResponse> SendMessage(SendMessageRequest message)
    {
        var result = await _mediator.Send(new SendMessageCommand(message));
        
        // TODO handle failure
        return result.Value;
    }
}