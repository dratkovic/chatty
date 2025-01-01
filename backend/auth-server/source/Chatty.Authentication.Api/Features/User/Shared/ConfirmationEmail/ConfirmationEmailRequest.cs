using Chatty.Authentication.Api.Domain;
using Chatty.Core.Application.Common.Authorization;
using ErrorOr;
using MediatR;

namespace Chatty.Authentication.Api.Features.User.Shared.ConfirmationEmail;

[AllowGuests]
public record ConfirmationEmailRequest(AppUser User): IRequest<ErrorOr<Success>>{}
