using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Authorization;
using MediatR;

namespace Chatty.Application.Features.Group;

[DAuthorize]
public record GetUserGroupsQuery() : IRequest<ErrorOr<List<GroupResponse>>>;