using System.Text;
using System.Text.Encodings.Web;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Infrastructure.Email;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.Authentication.Api.Features.User.Shared.ConfirmationEmail;

public class ConfirmationEmailRequestHandler: IRequestHandler<ConfirmationEmailRequest, ErrorOr<Success>>
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IEmailSender _emailSender;
    private readonly UserManager<AppUser> _userManager;

    public ConfirmationEmailRequestHandler(LinkGenerator linkGenerator,
        IHttpContextAccessor contextAccessor,
        IEmailSender emailSender, 
        UserManager<AppUser> userManager)
    {
        _linkGenerator = linkGenerator;
        _contextAccessor = contextAccessor;
        _emailSender = emailSender;
        _userManager = userManager;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(request.User);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await _userManager.GetUserIdAsync(request.User);
        var routeValues = new RouteValueDictionary()
        {
            ["userId"] = userId,
            ["code"] = code,
        };
        
        var confirmEmailUrl = _linkGenerator.GetUriByName(_contextAccessor.HttpContext!, "ConfirmEmail", routeValues)
                              ?? throw new NotSupportedException("Could not find endpoint named 'ConfirmEmail'.");

        await _emailSender.SendConfirmationLinkAsync(request.User, HtmlEncoder.Default.Encode(confirmEmailUrl));

        return new Success();
    }
}