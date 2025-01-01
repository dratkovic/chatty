using Chatty.Application.Common.Authorization;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using ErrorOr;
using MediatR;

namespace Chatty.Application.Message.Commands;

[Authorize]
public sealed record SendMessageCommand(SendMessageRequest Request) 
    : IRequest<ErrorOr<MessageStatusResponse>>
{ }