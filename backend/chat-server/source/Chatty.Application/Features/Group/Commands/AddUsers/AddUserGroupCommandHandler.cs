using Chatty.Application.Common.Helpers;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Group;

public class AddUserGroupCommandHandler : IRequestHandler<AddUserToGroupCommand, ErrorOr<Success>>
{
    private readonly IUserRetriever _userRetriever;
    private readonly ILogger<AddUserGroupCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public AddUserGroupCommandHandler(IUserRetriever userRetriever,
        ILogger<AddUserGroupCommandHandler> logger,
        IAppDbContext dbContext)
    {
        _userRetriever = userRetriever;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await _userRetriever.GetCurrentUser(cancellationToken);

        if (currentUser is null)
        {
            _logger.LogWarning("User not found even though it should be authenticated.");
            return Error.Unauthorized();
        }

        var users = await _dbContext.Set<User>()
            .Where(x => request.Request.Users.Contains(x.Id))
            .ToListAsync(cancellationToken: cancellationToken);
        
        if(users.Count != request.Request.Users.Count)
        {
            return Error.Validation(description: "Invalid user ids");
        }
        
        var group = await _dbContext.Set<Domain.Group>()
            .Include(x => x.Participants)
            .FirstOrDefaultAsync(x => x.Id == request.Request.GroupId, cancellationToken);
        
        if(group is null)
        {
            return Error.NotFound();
        }

        var response = currentUser.AddUsersToGroup(users, group);
        
        if(response.IsError)
        {
            return Error.Validation(description: response.Errors.First().Description);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new Success();
    }
}