using Chatty.Authentication.Api.Contracts;
using Chatty.Authentication.Api.Features.User.Login;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Contracts;
using Chatty.Core.Contracts.Routes;
using MediatR;
using ErrorOr;
using FluentValidation.Results;

namespace Chatty.Authentication.Api.Features.User.Refresh;

public class RefreshEndpoint: IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(AuthenticationApiRoutes.Authentication.Refresh, RefreshAsync)
            .WithName("Refresh")
            .Accepts<LoginRequest>(ChattyApiConstants.JsonContentType)
            .Produces<LoginResponse>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Authentication);
    }
    
    private static async Task<IResult> RefreshAsync(ISender sender, RefreshRequest request, CancellationToken ct)
    {
        var result = await sender.Send(request, ct);
        
        return result.Match(Results.Ok,r=>r.HandleErrors());
    }
}

[AllowGuests]
public record RefreshRequest(string Token, string RefreshToken) : IRequest<ErrorOr<LoginResponse>>;