using Chatty.Authentication.Api.Contracts;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts.Routes;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chatty.Authentication.Api.Features.User.ConfirmEmail;

public class ConfirmEmailEndpoint: IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(AuthenticationApiRoutes.Authentication.ConfirmEmail, ConfirmEmailAsync)
            .WithName("ConfirmEmail")
            .Produces(200)
            .Produces(401)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> ConfirmEmailAsync(
        [FromQuery] string userId, [FromQuery] string code,
        ISender sender, CancellationToken cancellationToken)
    {
        var request = new ConfirmEmailRequest(userId, code);
        var response = await sender.Send(request, cancellationToken);
        return response.Match(Results.Ok, r=>r.HandleErrors());
    }
}

[AllowGuests]
public record ConfirmEmailRequest(string UserId, string Code): IRequest<ErrorOr<Success>>
{}