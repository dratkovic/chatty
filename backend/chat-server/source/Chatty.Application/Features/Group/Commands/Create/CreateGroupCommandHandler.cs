using Chatty.Application.Common.Helpers;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Persistance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chatty.Application.Features.Group;

public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, ErrorOr<GroupResponse>>
{
    private readonly IUserRetriever _userRetriever;
    private readonly ILogger<CreateGroupCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public CreateGroupCommandHandler(IUserRetriever userRetriever,
        ILogger<CreateGroupCommandHandler> logger,
        IAppDbContext dbContext)
    {
        _userRetriever = userRetriever;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GroupResponse>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await _userRetriever.GetCurrentUser(cancellationToken);

        if (currentUser is null)
        {
            _logger.LogWarning("User not found even though it should be authenticated.");
            return Error.Unauthorized();
        }

        var group = currentUser.CreateGroup(request.Request.Name, request.Request.IsPublic);
        
        if(group.IsError)
        {
            return Error.Validation(description: group.Errors.First().Description);
        }
        
        _dbContext.Set<Domain.Group>().Add(group.Value);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new GroupResponse(group.Value.Id, group.Value.Name, group.Value.IsPublic);
    }
}