using Chatty.Application.Common.Helpers;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Persistance;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Message.Commands;

internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ErrorOr<MessageStatusResponse>>
{
    private readonly IUserRetriever _userSession;
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<SendMessageCommandHandler> _logger;

    public SendMessageCommandHandler(IUserRetriever userSession, IAppDbContext dbContext, ILogger<SendMessageCommandHandler> logger)
    {
        _userSession = userSession;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ErrorOr<MessageStatusResponse>> Handle(SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userSession.GetCurrentUser();

        if (user == null)
        {
            _logger.LogUnauthenticatedUserIsTryingTo("Send a message");
            return Error.Validation("User not found");
        }

        var message = user.SendMessage(request.Request.Content,
            request.Request.ReceiverId, request.Request.GroupId);

        if (message.IsError)
        {
            _logger.LogInformation("Message failed to send to chatty: {0}", message.FirstError.Description);
            return Error.Validation(description: message.FirstError.Description);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MessageStatusResponse(message.Value.Id, message.Value.RecipientId, message.Value.GroupId,
            message.Value.TimeStampUtc, "Sent");
    }
}