using Chatty.Authentication.Api.Domain;

namespace Chatty.Authentication.Api.Infrastructure.Email;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    Task SendConfirmationLinkAsync(AppUser user, string encodedLink);
    Task SendPasswordResetCodeAsync(AppUser user, string encodedLink);
}