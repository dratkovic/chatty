using FluentValidation;

namespace Chatty.Application.Features.Group;

public class RemoveUsersFromGroupValidator: AbstractValidator<RemoveUsersFromGroupCommand>
{
    public RemoveUsersFromGroupValidator()
    {
        RuleFor(x => x.Request.Users)
            .NotEmpty();

        RuleFor(x => x.Request.GroupId)
            .NotEmpty();
    }
}