using Chatty.Domain;
using Xunit;

namespace Chatty.Domain.Tests.Unit;

public class GroupTests
{
    [Fact]
    public void CreateGroup_ShouldAddCreatorAsParticipantAndAdmin()
    {
        // Arrange
        var creator = User.Create("creator@example.com", "Creator User").Value;
        var groupName = "Test Group";
        var isPublic = true;

        // Act
        var group = creator.CreateGroup(groupName, isPublic);

        // Assert
        Assert.Contains(creator, group.Value.Participants);
        Assert.Contains(creator, group.Value.Admins);
    }

    [Fact]
    public void CreateGroup_WithEmptyName_ShouldSetNameToNewGroup()
    {
        // Arrange
        var creator = User.Create("creator@example.com", "Creator User").Value;
        var groupName = string.Empty;
        var isPublic = true;

        // Act
        var group = creator.CreateGroup(groupName, isPublic);

        // Assert
        Assert.Equal("New Group", group.Value.Name);
    }
}