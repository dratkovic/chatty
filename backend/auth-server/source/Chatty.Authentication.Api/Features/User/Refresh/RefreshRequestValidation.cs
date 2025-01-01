using FluentValidation;

namespace Chatty.Authentication.Api.Features.User.Refresh;

public class RefreshRequestValidation: AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidation()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("refresh token is required!");
        
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("refresh token is required!");
    }
}