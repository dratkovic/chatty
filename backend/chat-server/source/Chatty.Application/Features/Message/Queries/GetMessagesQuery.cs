using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Authorization;
using ErrorOr;
using MediatR;

namespace Chatty.Application.Features.Message.Queries;

[DAuthorize]
public record GetMessagesQuery(
    MessagesQuery Query) : IRequest<ErrorOr<List<MessageResponse>>>;