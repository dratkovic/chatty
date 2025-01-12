using System.Net;
using Chatty.Contracts.Requests;
using Chatty.Contracts.Responses;
using Chatty.webApi.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace Chatty.WebApi.Tests.Integration.Features.Messages;

[Collection("Chatty tests")]
public class MessagesHttpTests : IAsyncLifetime
{
    private readonly ChattyApiFactory _factory;

    public MessagesHttpTests(ChattyApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SendMessage_ShouldBeOk_WhenValidRecipient()
    {
        // Arrange
        var request = new SendMessageRequest("Hello", _factory.User1.Id, null);
        _factory.SetAuthorizationHeader(_factory.User1);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(ApiConstants.Routes.Messages.MessagesBase, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendMessage_ShouldFail_WhenSendingBothToGroupAndFriend()
    {
        // Arrange
        var request = new SendMessageRequest("Hello", _factory.User1.Id, _factory.User2.Id);
        _factory.SetAuthorizationHeader(_factory.User1);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(ApiConstants.Routes.Messages.MessagesBase, request);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("Request");
        error.description.Should().Be("Cannot have both GroupId and ReceiverId set");
    }

    [Fact]
    public async Task SendMessage_ShouldFail_WhenSendingToNotExistingGroup()
    {
        // Arrange
        var request = new SendMessageRequest("Hello", null, Guid.NewGuid());
        _factory.SetAuthorizationHeader(_factory.User1);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(ApiConstants.Routes.Messages.MessagesBase, request);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("General.Validation");
        error.description.Should().Be("User is not a member of the group");
    }

    [Fact]
    public async Task SendMessage_ShouldFail_WhenFriendAndGroupNotSet()
    {
        // Arrange
        var request = new SendMessageRequest("Hello", null, null);
        _factory.SetAuthorizationHeader(_factory.User1);

        // Act
        var response = await _factory.HttpClient.PostAsJsonAsync(ApiConstants.Routes.Messages.MessagesBase, request);
        var validationFailures = await response.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var validationErrors = validationFailures as ValidationError[] ?? validationFailures!.ToArray();
        var error = validationErrors.FirstOrDefault();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.code.Should().Be("Request");
        error.description.Should().Be("ReceiverId or GroupId must be set");
    }

    [Fact]
    public async Task User2_ShouldGetMessages_WhenUser1SendsMessageToHim()
    {
        // Arrange
        var sendMessageRequest = new SendMessageRequest("Hello User2", _factory.User2.Id, null);
        _factory.SetAuthorizationHeader(_factory.User1);

        // Act
        var sendResponse =
            await _factory.HttpClient.PostAsJsonAsync(ApiConstants.Routes.Messages.MessagesBase, sendMessageRequest);
        sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Set authorization for User2 to get messages
        _factory.SetAuthorizationHeader(_factory.User2);

        // Act
        var baseUrl = ApiConstants.Routes.Messages.MessagesBase;
        var queryParams = new Dictionary<string, string>
        {
            { "FriendId", _factory.User1.Id.ToString() }
        };

        var urlWithQuery = QueryHelpers.AddQueryString(baseUrl, queryParams);
        var getResponse = await _factory.HttpClient.GetAsync(urlWithQuery);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var messages = await getResponse.Content.ReadFromJsonAsync<IEnumerable<MessageResponse>>();
        var message = messages.FirstOrDefault(m => m.Content == "Hello User2");

        // Assert
        message.Should().NotBeNull();
        message!.SenderId.Should().Be(_factory.User1.Id);
    }


    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        _factory.ResetHttpClient();
    }
}