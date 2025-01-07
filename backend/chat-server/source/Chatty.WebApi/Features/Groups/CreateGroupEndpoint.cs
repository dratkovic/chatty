using Chatty.Application.Features.Group;
using Chatty.Application.Features.Message.Commands;
using Chatty.Contracts.Requests;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.webApi.Contracts;
using FluentValidation.Results;
using MediatR;

namespace Chatty.webApi.Features.Groups;

public class CreateGroupEndpoint: IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiConstants.Routes.Groups.GroupsBase, CreateGroup)
            .WithName("CreateGroup")
            .Accepts<CreateGroupRequest>(ApiConstants.JsonContentType)
            .Produces(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Groups)
            .RequireAuthorization();
    }

    private static async Task<IResult> CreateGroup(ISender sender, CreateGroupRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateGroupCommand(request), ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}