using Chatty.Application.Common.Helpers;
using Chatty.Core.Application.Common.Persistance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Group;

public class LeaveGroupCommandHandler : IRequestHandler<LeaveGroupCommand, ErrorOr<Success>>
{
    private readonly IUserRetriever _userRetriever;
    private readonly ILogger<LeaveGroupCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public LeaveGroupCommandHandler(IUserRetriever userRetriever,
        ILogger<LeaveGroupCommandHandler> logger,
        IAppDbContext dbContext)
    {
        _userRetriever = userRetriever;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(LeaveGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await _userRetriever.GetCurrentUser(cancellationToken);

        if (currentUser is null)
        {
            _logger.LogWarning("User not found even though it should be authenticated.");
            return Error.Unauthorized();
        }
        
        var group = await _dbContext.Set<Domain.Group>()
            .Include(x => x.Participants)
            .Include(x=>x.Admins)
            .FirstOrDefaultAsync(x => x.Id == request.GroupId, cancellationToken);
        
        if(group is null)
        {
            return Error.NotFound();
        }

        var response = currentUser.LeaveGroup(group);
        
        if(response.IsError)
        {
            return Error.Validation(description: response.Errors.First().Description);
        }
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new Success();
    }
}