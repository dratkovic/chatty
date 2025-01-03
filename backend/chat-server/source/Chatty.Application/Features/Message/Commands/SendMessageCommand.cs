using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.Core.Application.Common.Authorization;
using ErrorOr;
using MediatR;

namespace Chatty.Application.Features.Message.Commands;

[DAuthorize]
public sealed record SendMessageCommand(SendMessageRequest Request) 
    : IRequest<ErrorOr<MessageStatusResponse>>
{ }