using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Authorization;
using MediatR;

namespace Chatty.Application.Features.Group;

[DAuthorize]
public record GetParticipantsQuery(
    Guid GroupId) : IRequest<ErrorOr<List<UserResponse>>>;