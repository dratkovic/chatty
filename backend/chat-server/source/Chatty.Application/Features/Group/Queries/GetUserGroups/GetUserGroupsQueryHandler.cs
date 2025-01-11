using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Group;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, ErrorOr<List<GroupResponse>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;
    private readonly ILogger<GetUserGroupsQueryHandler> _logger;

    public GetUserGroupsQueryHandler(IAppDbContext dbContext,
        IAuthenticatedUserProvider authenticatedUserProvider,
        ILogger<GetUserGroupsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _authenticatedUserProvider = authenticatedUserProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<List<GroupResponse>>> Handle(GetUserGroupsQuery request,
        CancellationToken cancellationToken)
    {
        var user = _authenticatedUserProvider.GetCurrentUser();

        try
        {
            var groups = await _dbContext.Set<User>()
                .Where(x => x.Id == user.Id)
                .SelectMany(x => x.Groups)
                .Select(y => new GroupResponse(y.Id, y.Name, y.IsPublic, y.Admins.Any(z => z.Id == user.Id)))
                .ToListAsync(cancellationToken);

            return groups;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get groups");
        }

        return Error.Unexpected();
    }
}