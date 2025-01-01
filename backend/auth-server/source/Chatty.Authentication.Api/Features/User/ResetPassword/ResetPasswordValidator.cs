using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.ResetPassword;

public class ResetPasswordValidator: AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("a password is required!")
            .MinimumLength(8).WithMessage("password is too short!")
            .MaximumLength(25).WithMessage("password is too long!");

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}