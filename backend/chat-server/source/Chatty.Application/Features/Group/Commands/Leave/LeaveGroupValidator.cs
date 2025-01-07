using FluentValidation;

namespace Chatty.Application.Features.Group;

public class LeaveGroupValidator: AbstractValidator<LeaveGroupCommand>
{
    public LeaveGroupValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty();
    }
}