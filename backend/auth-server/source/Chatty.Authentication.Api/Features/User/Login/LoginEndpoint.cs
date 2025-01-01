using Chatty.Authentication.Api.Contracts;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts;
using Chatty.Core.Contracts.Routes;
using ErrorOr;
using FluentValidation.Results;
using MediatR;

namespace Chatty.Authentication.Api.Features.User.Login;

public class LoginEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthenticationApiRoutes.Authentication.Login, LoginAsync)
            .WithName("Login")
            .Accepts<LoginRequest>(ChattyApiConstants.JsonContentType)
            .Produces<LoginResponse>()
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> LoginAsync(ISender sender, LoginRequest request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}

[AllowGuests]
public record LoginRequest(string Email, string Password) : IRequest<ErrorOr<LoginResponse>>
{
}

public record LoginResponse(
    string Email,
    string FirstName,
    string LastName,
    string Token,
    string RefreshToken)
{
}