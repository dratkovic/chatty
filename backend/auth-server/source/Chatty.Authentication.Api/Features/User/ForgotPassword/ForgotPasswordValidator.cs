using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.ForgotPassword;

public class ForgotPasswordValidator: AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}