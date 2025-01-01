using Chatty.Authentication.Api.Contracts;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts;
using Chatty.Core.Contracts.Routes;
using ErrorOr;
using FluentValidation.Results;
using MediatR;

namespace Chatty.Authentication.Api.Features.User.ResetPassword;

public class ResetPasswordEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthenticationApiRoutes.Authentication.ResetPassword, RestPasswordAsync)
            .WithName("ResetPassword")
            .Accepts<LoginRequest>(ChattyApiConstants.JsonContentType)
            .Produces(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> RestPasswordAsync(ResetPasswordRequest request, ISender sender)
    {
        var response = await sender.Send(request);

        return response.Match(
            r => Results.Ok(),
            Results.BadRequest);
    }
}

[AllowGuests]
public record ResetPasswordRequest(string Email, string NewPassword, string Code)
    : IRequest<ErrorOr<Success>>;