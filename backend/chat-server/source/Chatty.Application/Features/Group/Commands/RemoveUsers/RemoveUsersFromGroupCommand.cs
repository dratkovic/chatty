using Chatty.Contracts.Requests;
using Chatty.Core.Application.Common.Authorization;
using MediatR;

namespace Chatty.Application.Features.Group;

[DAuthorize]
public sealed record RemoveUsersFromGroupCommand(
    UsersGroupRequest Request)
    : IRequest<ErrorOr<Success>>
{
}