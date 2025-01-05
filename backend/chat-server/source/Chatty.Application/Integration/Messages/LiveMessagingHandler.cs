using Chatty.Application.Services;
using Chatty.Contracts.Responses;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Integration.Messages;

public class LiveMessagingHandler : IConsumer<MessageResponse>
{
    private readonly ILiveChatService _liveChatService;
    private readonly ILogger<LiveMessagingHandler> _logger;

    public LiveMessagingHandler(ILiveChatService liveChatService, ILogger<LiveMessagingHandler> logger)
    {
        _liveChatService = liveChatService;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MessageResponse> context)
    {
        try
        {
            _logger.LogInformation("Delivering message to user");
            _liveChatService.DeliverMessageToUser(context.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deliver message to user");
        }

        // TODO handle failure
        return Task.CompletedTask;
    }
}