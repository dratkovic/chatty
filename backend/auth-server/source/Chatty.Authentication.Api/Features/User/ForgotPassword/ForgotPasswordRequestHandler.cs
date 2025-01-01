using System.Text;
using System.Text.Encodings.Web;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Infrastructure.Email;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.Authentication.Api.Features.User.ForgotPassword;

public class ForgotPasswordRequestHandler: IRequestHandler<ForgotPasswordRequest, ErrorOr<Success>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordRequestHandler(UserManager<AppUser> userManager, IEmailSender emailSender)
    {
        _userManager = userManager;
        _emailSender = emailSender;
    }

    public async Task<ErrorOr<Success>> Handle(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await _emailSender.SendPasswordResetCodeAsync(user, HtmlEncoder.Default.Encode(code));
        }

        // not sending 400 to not expose if user exists or not
        return new Success();
    }
}