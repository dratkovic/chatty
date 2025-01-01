using Chatty.Authentication.Api.Domain;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Models;
using MediatR;
using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Features.User.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ErrorOr<LoginResponse>>
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenManipulator _jwtTokenManipulator;

    public LoginCommandHandler(SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        IJwtTokenManipulator jwtTokenManipulator)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenManipulator = jwtTokenManipulator;
    }

    public async Task<ErrorOr<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if(user is null)
        {
            return Error.Unauthorized();
        }
        
        var userRoles = await _userManager.GetRolesAsync(user!);

        var token = _jwtTokenManipulator.GenerateToken(new AuthenticationUser(user!.Id,
            user.Email!.ToLower(),
            user.FirstName,
            user.LastName,
            userRoles.ToList()));
        
        var refreshToken = _jwtTokenManipulator.GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(_jwtTokenManipulator.RefreshTokenExpirationInHours);
        
        await _userManager.UpdateAsync(user);
        
        return new LoginResponse(user.Email!.ToLower(), user.FirstName, user.LastName, token, refreshToken);
    }
}

[AllowGuests]
public record LoginCommand(string Email) : IRequest<ErrorOr<LoginResponse>>;