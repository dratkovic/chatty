using System.Text;
using Chatty.Authentication.Api.Domain;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.Authentication.Api.Features.User.ConfirmEmail;

public class ConfirmEmailHandler: IRequestHandler<ConfirmEmailRequest, ErrorOr<Success>>
{
    private readonly UserManager<AppUser> userManager;

    public ConfirmEmailHandler(UserManager<AppUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return Error.Unauthorized();
        }

        string code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
        }
        catch (FormatException)
        {
            return Error.Unauthorized();
        }

        var result = await userManager.ConfirmEmailAsync(user, code);
        
        if (!result.Succeeded)
        {
            return Error.Unauthorized();
        }

        return new Success();
    }
}