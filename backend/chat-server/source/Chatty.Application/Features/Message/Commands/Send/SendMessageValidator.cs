using Chatty.Contracts.Requests;
using FluentValidation;

namespace Chatty.Application.Features.Message.Commands;

public class SendMessageValidator: AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Request)
            .Must(HaveFriendOrGroupSet)
            .WithMessage("ReceiverId or GroupId must be set");
        
        RuleFor(x => x.Request)
            .Must(CannotHaveBothGroupAndFriendSet)
            .WithMessage("Cannot have both GroupId and ReceiverId set");

        RuleFor(x => x.Request.Content)
            .NotEmpty();
    }

    private bool HaveFriendOrGroupSet(SendMessageRequest arg)
    {
        return arg.ReceiverId.HasValue || arg.GroupId.HasValue;
    }
    
    private bool CannotHaveBothGroupAndFriendSet(SendMessageRequest arg)
    {
        return arg is not { ReceiverId: not null, GroupId: not null };
    }
}