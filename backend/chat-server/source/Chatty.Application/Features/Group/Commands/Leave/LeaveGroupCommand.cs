using Chatty.Core.Application.Common.Authorization;
using MediatR;

namespace Chatty.Application.Features.Group;

[DAuthorize]
public sealed record LeaveGroupCommand(
    Guid GroupId)
    : IRequest<ErrorOr<Success>>
{
}