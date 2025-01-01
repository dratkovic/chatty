using Chatty.Authentication.Api.Domain;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Features.User.Login;

public class LoginRequestHandler : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    private readonly SignInManager<AppUser> _signInManager;
    private ISender _sender;
    

    public LoginRequestHandler(SignInManager<AppUser> signInManager, 
        ISender sender)
    {
        _signInManager = signInManager;
        _sender = sender;
    }

    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var result =
            await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return Error.Validation(code: "General:Authorization", 
                    description: "User account is locked out");
            }
            else if (result.IsNotAllowed)
            {
                return Error.Validation(code: "General:Authorization", 
                    description: "Account is not confirmed");
            }
            else if (result.RequiresTwoFactor)
            {
                return Error.Validation(code: "General:Authorization",
                    description: "User account requires two factor authentication");
            }
            else
            {
                return Error.Unauthorized();
            }
        }

        return await _sender.Send(new LoginCommand(request.Email), cancellationToken);
    }
}