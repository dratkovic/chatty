using System.Security.Claims;
using Chatty.Application.Features.Message.Commands;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chatty.Infrastructure.LiveChat;

[Authorize]
public class LiveChatHub : Hub<ILiveChatClient>, ILiveChatServer
{
    private readonly ISender _mediator;

    public LiveChatHub(ISender mediator)
    {
        _mediator = mediator;
    }

    public override Task OnConnectedAsync()
    {
        Clients.All.ReceiveMessage(new MessageResponse(Guid.NewGuid(),
            Guid.NewGuid(),
            "Damir the Tester",
            "Hello World",
            Guid.NewGuid(),
            null,
            DateTime.Now,
            "sent"));
        return base.OnConnectedAsync();
    }

    public async Task<MessageStatusResponse> SendMessage(SendMessageRequest message)
    {
        var result = await _mediator.Send(new SendMessageCommand(message));

        // TODO handle failure
        return result.Value;
    }
}