using Chatty.Domain;
using ErrorOr;
using Xunit;

namespace Chatty.Domain.Tests.Unit;

public class MessageTests
{
    [Fact]
    public void Create_ShouldReturnError_WhenContentIsEmpty()
    {
        // Arrange
        var user = User.Create("sender@example.com", "Sender User").Value;
        var content = string.Empty;
        Guid? groupId = null;
        Guid? recipientId = Guid.NewGuid();

        // Act
        var result = user.SendMessage(content, groupId, recipientId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message content cannot be empty", result.FirstError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenRecipientIdAndGroupIdAreNull()
    {
        // Arrange
        var user = User.Create("sender@example.com", "Sender User").Value;
        var content = "Test message";
        Guid? groupId = null;
        Guid? recipientId = null;

        // Act
        var result = user.SendMessage(content, groupId, recipientId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message must have a recipient or a group", result.FirstError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenRecipientIdAndGroupIdAreBothPopulated()
    {
        // Arrange
        var user = User.Create("sender@example.com", "Sender User").Value;
        var group = user.CreateGroup("test group").Value;
        var content = "Test message";
        Guid? groupId = group.Id;
        Guid? recipientId = Guid.NewGuid();

        // Act
        var result = user.SendMessage(content, groupId, recipientId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message cannot have both a recipient and a group", result.FirstError.Code);
    }


    [Fact]
    public void Create_ShouldReturnSuccess_WhenMessageIsValid()
    {
        // Arrange
        var user = User.Create("sender@example.com", "Sender User").Value;
        var group = user.CreateGroup("test group").Value;
        var content = "Valid message content";
        Guid? groupId = group.Id;
        Guid? recipientId = null;
        var beforeTimestamp = DateTime.UtcNow;

        // Act
        var result = user.SendMessage(content, groupId, recipientId);
        var afterTimestamp = DateTime.UtcNow;

        // Assert
        Assert.False(result.IsError);
        var message = result.Value;
        Assert.Equal(content, message.Content);
        Assert.Equal(groupId, message.GroupId);
        Assert.Equal(recipientId, message.RecipientId);
        Assert.Equal(user, message.Sender);
        Assert.InRange(message.TimeStampUtc, beforeTimestamp, afterTimestamp.AddMilliseconds(200));
    }
}