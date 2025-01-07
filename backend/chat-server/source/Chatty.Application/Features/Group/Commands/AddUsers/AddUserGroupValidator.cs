using FluentValidation;

namespace Chatty.Application.Features.Group;

public class AddUserGroupValidator: AbstractValidator<AddUserToGroupCommand>
{
    public AddUserGroupValidator()
    {
        RuleFor(x => x.Request.Users)
            .NotEmpty();

        RuleFor(x => x.Request.GroupId)
            .NotEmpty();
    }
}