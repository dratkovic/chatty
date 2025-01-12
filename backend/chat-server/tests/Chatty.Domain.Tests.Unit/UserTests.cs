using Chatty.Domain;
using Chatty.Domain.Events;
using ErrorOr;
using Xunit;

namespace Chatty.Domain.Tests.Unit;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnUser_WhenEmailIsValid()
    {
        // Arrange
        var email = "test@example.com";
        var displayName = "Test User";

        // Act
        var result = User.Create(email, displayName);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(displayName, result.Value.DisplayName);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEmailIsInvalid()
    {
        // Arrange
        var email = "invalid-email";
        var displayName = "Test User";

        // Act
        var result = User.Create(email, displayName);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Invalid email address", result.FirstError.Description);
    }

    [Fact]
    public void CreateGroup_ShouldReturnGroup_WhenNameIsValid()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var groupName = "Test Group";

        // Act
        var result = user.CreateGroup(groupName);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value);
        Assert.Equal(groupName, result.Value.Name);
    }

    [Fact]
    public void SendMessage_ShouldReturnError_WhenNoRecipientOrGroup()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;

        // Act
        var result = user.SendMessage("Hello", null, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message must have a recipient or a group", result.FirstError.Code);
    }

    [Fact]
    public void SendMessage_ShouldReturnError_WhenBothRecipientAndGroupProvided()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group").Value;
        var groupId = group.Id;
        var recipientId = Guid.NewGuid();

        // Act
        var result = user.SendMessage("Hello", groupId, recipientId);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Message cannot have both a recipient and a group", result.FirstError.Code);
    }

    [Fact]
    public void SendMessage_ShouldReturnError_WhenUserNotInGroup()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var groupId = Guid.NewGuid();

        // Act
        var result = user.SendMessage("Hello", groupId, null);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User is not a member of the group", result.FirstError.Description);
    }

    [Fact]
    public void SendMessage_ShouldAddDomainEvent_WhenMessageIsValid()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group").Value;

        // Act
        var result = user.SendMessage("Hello", group.Id, null);

        // Assert
        Assert.False(result.IsError);
        var events = user.PopDomainEvents();
        Assert.Contains(events, e => e is MessagePostedEvent);
    }

    [Fact]
    public void CreateGroup_ShouldAddDomainEvent_WhenGroupIsCreated()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var groupName = "Test Group";

        // Act
        var result = user.CreateGroup(groupName);

        // Assert
        Assert.False(result.IsError);
        var events = user.PopDomainEvents();
        Assert.Contains(events, e => e is GroupCreatedEvent);
    }

    [Fact]
    public void JoinGroup_ShouldReturnError_WhenGroupIsPrivate()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group", isPublic: false).Value;
        var user1 = User.Create("test1@example.com", "Test User 1").Value;

        // Act
        var result = user1.JoinGroup(group);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Group is private", result.FirstError.Description);
    }

    [Fact]
    public void JoinGroup_ShouldAddDomainEvent_WhenGroupIsPublic()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group", isPublic: true).Value;
        var user1 = User.Create("test1@example.com", "Test User 1").Value;

        // Act
        var result = user1.JoinGroup(group);

        // Assert
        Assert.False(result.IsError);
        var events = user1.PopDomainEvents();
        Assert.Contains(events, e => e is UserJoinedGroupEvent);
    }

    [Fact]
    public void LeaveGroup_ShouldAddDomainEvent_WhenUserLeavesGroup()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group", isPublic: true).Value;
        var user2 = User.Create("test2@example.com", "Test User 2").Value;
        user2.JoinGroup(group);

        // Act
        var result = user2.LeaveGroup(group);

        // Assert
        Assert.False(result.IsError);
        var events = user2.PopDomainEvents();
        Assert.Contains(events, e => e is UserLeftGroupEvent);
    }

    [Fact]
    public void RemoveUserFromGroup_ShouldAddDomainEvent_WhenAdminRemovesUser()
    {
        // Arrange
        var admin = User.Create("admin@example.com", "Admin User").Value;
        var group = admin.CreateGroup("Test Group", isPublic: true).Value;
        var user2 = User.Create("user2@example.com", "User 2").Value;
        admin.AddUsersToGroup(new List<User> { user2 }, group);

        // Act
        var result = admin.RemoveUsersFromGroup(new List<User> { user2 }, group);

        // Assert
        Assert.False(result.IsError);
        var events = admin.PopDomainEvents();
        Assert.Contains(events, e => e is UserRemovedFromGroupEvent);
    }

    [Fact]
    public void AddUsersToGroup_ShouldReturnError_WhenUserIsNotAdmin()
    {
        // Arrange
        var admin = User.Create("admin@example.com", "Admin User").Value;
        var group = admin.CreateGroup("Test Group", isPublic: true).Value;
        var user = User.Create("user@example.com", "User").Value;
        var user2 = User.Create("user2@example.com", "User 2").Value;

        // Act
        var result = user.AddUsersToGroup(new List<User> { user2 }, group);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User is not an admin of the group", result.FirstError.Description);
    }

    [Fact]
    public void RemoveUsersFromGroup_ShouldReturnError_WhenUserIsNotAdmin()
    {
        // Arrange
        var admin = User.Create("admin@example.com", "Admin User").Value;
        var group = admin.CreateGroup("Test Group", isPublic: true).Value;
        var user = User.Create("user@example.com", "User").Value;
        var user2 = User.Create("user2@example.com", "User 2").Value;
        admin.AddUsersToGroup(new List<User> { user2 }, group);

        // Act
        var result = user.RemoveUsersFromGroup(new List<User> { user2 }, group);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User is not an admin of the group", result.FirstError.Description);
    }

    [Fact]
    public void LeaveGroup_ShouldReturnError_WhenUserIsNotInGroup()
    {
        // Arrange
        var user = User.Create("test@example.com", "Test User").Value;
        var group = user.CreateGroup("Test Group", isPublic: true).Value;
        var user2 = User.Create("test2@example.com", "Test User 2").Value;

        // Act
        var result = user2.LeaveGroup(group);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("User is not a member of the group", result.FirstError.Description);
    }

    [Fact]
    public void LeaveGroup_ShouldReturnError_WhenUserIsOnlyAdmin()
    {
        // Arrange
        var admin = User.Create("admin@example.com", "Admin User").Value;
        var group = admin.CreateGroup("Test Group", isPublic: true).Value;

        // Act
        var result = admin.LeaveGroup(group);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Cannot leave group as the only admin", result.FirstError.Description);
    }
}