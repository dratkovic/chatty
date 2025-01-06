using Chatty.Application.Features.Message.Queries;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.webApi.Contracts;
using FluentValidation.Results;
using MediatR;

namespace Chatty.WebApi.Features.Messages;

public class GetMessageEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ApiConstants.Routes.Messages.MessagesBase, GetMessages)
            .WithName("GetMessages")
            .Produces<List<MessageResponse>>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Messages)
            .RequireAuthorization();
    }

    private static async Task<IResult> GetMessages([AsParameters] MessagesQuery query,
        ISender sender,
        CancellationToken ct)
    {
        var result = await sender.Send(new GetMessagesQuery(query), ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}