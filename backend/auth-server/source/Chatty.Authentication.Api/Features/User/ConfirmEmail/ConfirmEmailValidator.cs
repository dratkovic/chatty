using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.ConfirmEmail;

public class ConfirmEmailValidator: AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}