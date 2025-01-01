using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("email address is required!")
            .EmailAddress().WithMessage("the format of your email address is wrong!");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("a password is required!")
            .MinimumLength(8).WithMessage("password is too short!")
            .MaximumLength(25).WithMessage("password is too long!");

    }
}