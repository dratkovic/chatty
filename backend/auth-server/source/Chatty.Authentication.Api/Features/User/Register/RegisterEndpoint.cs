using Chatty.Authentication.Api.Contracts;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts;
using Chatty.Core.Contracts.Routes;
using MediatR;
using ErrorOr;
using FluentValidation.Results;

namespace Chatty.Authentication.Api.Features.User.Register;

public class RegisterEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthenticationApiRoutes.Authentication.Register, RegisterAsync)
            .WithName("Register")
            .Accepts<RegisterRequest>(ChattyApiConstants.JsonContentType)
            .Produces(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }

    private static async Task<IResult> RegisterAsync(RegisterRequest request, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);

        return result.Match(r => Results.Created(), r => r.HandleErrors());
    }
}

[AllowGuests]
public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<ErrorOr<Success>>
{
}
