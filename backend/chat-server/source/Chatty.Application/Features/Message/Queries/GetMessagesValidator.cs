using Chatty.Contracts.Requests;
using FluentValidation;

namespace Chatty.Application.Features.Message.Queries;

public class GetMessagesValidator: AbstractValidator<GetMessagesQuery>
{
    public GetMessagesValidator()
    {
        RuleFor(x => x.Query)
            .Must(HaveFriendOrGroupSet)
            .WithMessage("FriendId or GroupId must be set");
    }

    private bool HaveFriendOrGroupSet(MessagesQuery arg)
    {
        return arg.FriendId.HasValue || arg.GroupId.HasValue;
    }
}