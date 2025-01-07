using FluentValidation;

namespace Chatty.Application.Features.Group;

public class CreateGroupValidator: AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}