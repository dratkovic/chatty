using System.Security.Claims;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Core.Application.Common.Interfaces;
using MediatR;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Features.User.Refresh;

public class RefreshRequestHandler: IRequestHandler<RefreshRequest, ErrorOr<LoginResponse>>
{
    private readonly IJwtTokenManipulator _jwtTokenManipulator;
    private readonly ISender _sender;
    private readonly UserManager<AppUser> _userManager;

    public RefreshRequestHandler(IJwtTokenManipulator jwtTokenManipulator, ISender sender, UserManager<AppUser> userManager)
    {
        _jwtTokenManipulator = jwtTokenManipulator;
        _sender = sender;
        _userManager = userManager;
    }

    public async Task<ErrorOr<LoginResponse>> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var principalResponse = await _jwtTokenManipulator.GetPrincipalFromExpiredToken(request.Token);
        if(principalResponse.IsError)
        {
            return Error.Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(principalResponse.Value.Claims
            .First(x => x.Type == ClaimTypes.Email).Value);
        
        if(user is null || user.RefreshToken != request.RefreshToken)
        {
            return Error.Unauthorized();
        }

        return await _sender.Send(new LoginCommand(user.Email!), cancellationToken);
    }
}