using Chatty.Authentication.Api.Contracts;
using Chatty.Authentication.Api.Domain;
using Chatty.Authentication.Api.Features.User.Shared.ConfirmationEmail;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Contracts.Routes;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chatty.Authentication.Api.Features.User.ResendConfirmationEmail;

public class ResendConfirmationEmailEndpoint: IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(AuthenticationApiRoutes.Authentication.ResendConfirmEmail, ResendConfirmEmailAsync)
            .WithName("ResendConfirmEmail")
            .Produces(200)
            .Produces(404)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> ResendConfirmEmailAsync(
        [FromQuery] string email, ISender sender, UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            // not sending 400 to not expose if user exists or not
            return TypedResults.Ok();
        }

        var result = await sender.Send(new ConfirmationEmailRequest(user));
        return result.Match(Results.Ok, Results.NotFound);
    }
}