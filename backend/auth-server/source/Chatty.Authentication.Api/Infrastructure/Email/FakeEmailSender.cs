using Chatty.Authentication.Api.Domain;

namespace Chatty.Authentication.Api.Infrastructure.Email;

public class FakeEmailSender : IEmailSender
{
    private readonly ILogger<FakeEmailSender> _logger;

    public FakeEmailSender(ILogger<FakeEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation($"Sending email to {email} with subject {subject} and message {htmlMessage}");
        return Task.CompletedTask;
    }

    public Task SendConfirmationLinkAsync(AppUser user, string encodedLink)
    {
        return SendEmailAsync(user.Email!, "Confirm your email", $"Dear {user.FirstName} {user.LastName}, Please confirm your email by clicking this link: {encodedLink}");
    }

    public Task SendPasswordResetCodeAsync(AppUser user, string encodedLink)
    {
        return SendEmailAsync(user.Email!, "Reset your password", $"Dear {user.FirstName} {user.LastName}, Please reset your password by clicking this link: {encodedLink}");
    }
}