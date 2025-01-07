using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Authorization;
using ErrorOr;
using MediatR;

namespace Chatty.Application.Features.Group;

[DAuthorize]
public sealed record CreateGroupCommand(
    CreateGroupRequest Request)
    : IRequest<ErrorOr<GroupResponse>>
{
}