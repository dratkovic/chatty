using Chatty.Application.Features.Group;
using Chatty.Contracts.Requests;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.webApi.Contracts;
using FluentValidation.Results;
using MediatR;

namespace Chatty.webApi.Features.Groups;

public class LeaveGroupEndpoint: IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiConstants.Routes.Groups.Leave+$"/{{id:Guid}}", LeaveGroup)
            .WithName("Leave")
            .Accepts<CreateGroupRequest>(ApiConstants.JsonContentType)
            .Produces(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Groups)
            .RequireAuthorization();
    }

    private static async Task<IResult> LeaveGroup(Guid id, ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new LeaveGroupCommand(id), ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}