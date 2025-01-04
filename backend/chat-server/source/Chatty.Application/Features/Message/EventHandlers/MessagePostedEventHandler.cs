using Chatty.Application.Services;
using Chatty.Domain.Events;
using MediatR;

namespace Chatty.Application.Features.Message.EventHandlers;

public class MessagePostedEventHandler: INotificationHandler<MessagePostedEvent>
{
    private readonly IMessageBroadcaster _messageBroadcaster;

    public MessagePostedEventHandler(IMessageBroadcaster messageBroadcaster)
    {
        _messageBroadcaster = messageBroadcaster;
    }

    public async Task Handle(MessagePostedEvent notification, CancellationToken cancellationToken)
    {
        await _messageBroadcaster.BroadcastMessage(notification.Message, cancellationToken);
    }
}