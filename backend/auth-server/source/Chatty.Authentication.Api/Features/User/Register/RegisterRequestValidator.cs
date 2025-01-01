using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.Register;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(3)
            .MaximumLength(25);

        RuleFor(x => x.LastName)
            .MinimumLength(3)
            .MaximumLength(25);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);
    }
}