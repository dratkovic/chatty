using Chatty.Application.Features.Group;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.webApi.Contracts;
using FluentValidation.Results;
using MediatR;

namespace Chatty.WebApi.Features.Groups;

public class GetUserGroupsEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiConstants.Routes.Groups.Mine, GetUserGroups)
            .WithName("GetMyGroups")
            .Produces<List<UserResponse>>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Groups)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetUserGroups([AsParameters] MessagesQuery query,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetUserGroupsQuery(), ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}