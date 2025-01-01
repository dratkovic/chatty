using System.Text;
using Chatty.Authentication.Api.Domain;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.Authentication.Api.Features.User.ResetPassword;

public class ResetPasswordHandler: IRequestHandler<ResetPasswordRequest, ErrorOr<Success>>
{
    private readonly UserManager<AppUser> _userManager;

    public ResetPasswordHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<Success>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        var error = _userManager.ErrorDescriber.InvalidToken();
        if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
            return Error.Validation(error.Code, error.Description);
        }

        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            return Error.Validation(error.Code, error.Description);
        }

        if (!result.Succeeded)
        {
            return Error.Validation(result.Errors.First().Code, result.Errors.First().Description);
        }

        return new Success();
    }
}