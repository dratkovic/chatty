using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using MediatR;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Message.Queries;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, ErrorOr<List<MessageResponse>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
    private readonly ILogger<GetMessagesQueryHandler> _logger;

    public GetMessagesQueryHandler(IAppDbContext dbContext,
        IAuthenticatedUserProvider authenticatedUserProvider,
        ILogger<GetMessagesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _authenticatedUserProvider = authenticatedUserProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<List<MessageResponse>>> Handle(GetMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var user = _authenticatedUserProvider.GetCurrentUser();

        var messagesQuery = _dbContext.Set<Domain.Message>()
            .AsNoTracking();

        if (request.Query.FriendId.HasValue)
        {
            messagesQuery = messagesQuery
                .Where(x => (x.SenderId == request.Query.FriendId && x.RecipientId == user.Id) ||
                            (x.SenderId == user.Id && x.RecipientId == request.Query.FriendId));
        }

        if (request.Query.GroupId.HasValue)
        {
            messagesQuery = messagesQuery
                .Where(x => x.GroupId == request.Query.GroupId);
        }

        if (request.Query.OlderThan.HasValue)
        {
            messagesQuery = messagesQuery
                .Where(x => x.TimeStampUtc < request.Query.OlderThan);
        }

        List<MessageResponse> messages;
        
        try
        {
            messages = await messagesQuery
                .OrderByDescending(x => x.TimeStampUtc)
                .Take(request.Query.Count)
                .Select(x => new MessageResponse(x.Id,
                    x.SenderId,
                    x.Sender.DisplayName,
                    x.Content,
                    x.RecipientId,
                    x.GroupId,
                    x.TimeStampUtc,
                    x.Status.ToString()))
                .ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get messages");
            return Error.Unexpected();
        }

        return messages;
    }
}