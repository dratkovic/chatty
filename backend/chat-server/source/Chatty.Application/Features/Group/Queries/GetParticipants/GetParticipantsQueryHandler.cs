using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Group;

public class GetParticipantsQueryHandler : IRequestHandler<GetParticipantsQuery, ErrorOr<List<UserResponse>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
    private readonly ILogger<GetParticipantsQueryHandler> _logger;

    public GetParticipantsQueryHandler(IAppDbContext dbContext,
        IAuthenticatedUserProvider authenticatedUserProvider,
        ILogger<GetParticipantsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _authenticatedUserProvider = authenticatedUserProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<List<UserResponse>>> Handle(GetParticipantsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var users = await _dbContext.Set<Domain.Group>()
                .Where(x => x.Id == request.GroupId)
                .SelectMany(x => x.Participants)
                .Select(y => new UserResponse(y.Id, y.DisplayName))
                .ToListAsync(cancellationToken);

            return users;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get messages");
        }

        return Error.Unexpected();
    }
}