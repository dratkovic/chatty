using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Shared.ConfirmationEmail;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Chatty.Authentication.Api.Features.User.Register;

public class RegisterRequestHandler(
    UserManager<AppUser> _userManager,
    ISender _sender)
    : IRequestHandler<RegisterRequest, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Error.Validation(result.Errors.First().Code, result.Errors.First().Description);
        }
        
        await _userManager.AddToRoleAsync(user, Roles.User);
        
        var sendConfirmationEmail = await _sender.Send(new ConfirmationEmailRequest(user));
        
        return sendConfirmationEmail;
    }
}