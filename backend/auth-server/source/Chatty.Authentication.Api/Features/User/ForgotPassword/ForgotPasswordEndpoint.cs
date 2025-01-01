using Chatty.Authentication.Api.Contracts;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts;
using Chatty.Core.Contracts.Routes;
using ErrorOr;
using FluentValidation.Results;
using MediatR;

namespace Chatty.Authentication.Api.Features.User.ForgotPassword;

public class ForgotPasswordEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthenticationApiRoutes.Authentication.ForgotPassword, ForgotPasswordAsync)
            .WithName("ForgotPassword")
            .Accepts<LoginRequest>(ChattyApiConstants.JsonContentType)
            .Produces(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request,
        ISender sender,
        CancellationToken ct)
    {
        var command = new ForgotPasswordRequest(request.Email);

        var response = await sender.Send(command, ct);

        return response.Match(
            _ => Results.Ok(),
            Results.BadRequest);
    }
}

[AllowGuests]
public record ForgotPasswordRequest(string Email) : IRequest<ErrorOr<Success>>
{
}