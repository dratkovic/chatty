using Chatty.Application.Common.Helpers;
using Chatty.Application.Common.Interfaces;
using Chatty.Application.Common.Repositories;
using Chatty.Contracts.Responses;
using MediatR;
using ErrorOr;

namespace Chatty.Application.Message.Commands;

internal class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ErrorOr<MessageStatusResponse>>
{
    private readonly IUserRetriever _userSession;
    private readonly IChattyDbContext _dbContext;

    public SendMessageCommandHandler(IUserRetriever userSession, IChattyDbContext dbContext)
    {
        _userSession = userSession;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<MessageStatusResponse>> Handle(SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userSession.GetCurrentUser();

        if (user == null)
        {
            return Error.Validation("User not found");
        }

        var message = user.SendMessage(request.Request.Content,
            request.Request.ReceiverId, request.Request.GroupId);

        if (message.IsError)
        {
            return Error.Validation(description: message.FirstError.Description);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new MessageStatusResponse(message.Value.Id, message.Value.RecipientId, message.Value.GroupId,
            message.Value.TimeStampUtc, "Sent");
    }
}