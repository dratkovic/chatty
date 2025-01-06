using Chatty.Application.Features.Message.Commands;
using Chatty.Contracts.Requests;
using Chatty.Core.Api;
using Chatty.Core.Api.EndPoints;
using Chatty.webApi.Contracts;
using FluentValidation.Results;
using MediatR;

namespace Chatty.WebApi.Features.Messages;

public class SendMessageEndpoint : IEndpoint
{
    public static void DefineEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ApiConstants.Routes.Messages.MessagesBase, SendMessage)
            .WithName("SendMessage")
            .Accepts<SendMessageRequest>(ApiConstants.JsonContentType)
            .Produces(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags(ApiConstants.EndpointTags.Messages)
            .RequireAuthorization();
    }

    private static async Task<IResult> SendMessage(ISender sender, SendMessageRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new SendMessageCommand(request), ct);

        return result.Match(Results.Ok, r => r.HandleErrors());
    }
}