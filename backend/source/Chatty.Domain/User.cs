using System.Net.Mail;
using Chatty.Domain.Common;
using Chatty.Domain.Events;
using ErrorOr;

namespace Chatty.Domain;

public class User: AggregateRoot
{
    public string Email { get; private set; }
    public string DisplayName { get; private set; }
    public List<Group> Groups { get; } = [];
    private User(string email, string displayName, Guid? id = null) : base(id)
    {
        Email = email;
        DisplayName = displayName;
    }

    public static ErrorOr<User> Create(string email, string displayName)
    {
        if (!IsEmailValid(email))
        {
            return  Error.Validation("Invalid email address");
        }
        var user = new User(email, displayName);
        user.AddDomainEvent(new UserCreatedEvent(user));
        return user;
    }
    
    public ErrorOr<Group> CreateGroup(string name, bool isPublic = false)
    {
        var groupName = string.IsNullOrEmpty(name) ? "New Group" : name;
        var group = new Group(this, groupName, isPublic);
        Groups.Add(group);
        AddDomainEvent(new GroupCreatedEvent(group));
        return group;
    }
    
    public ErrorOr<Success> JoinGroup(Group group)
    {
        if (group.IsPublic)
        {
            group.Participants.Add(this);
            AddDomainEvent(new UserJoinedGroupEvent(this, group));
            return new Success();
        }
        return Error.Validation("Group is private");
    }
    
    public ErrorOr<Success> LeaveGroup(Group group)
    {
        if (!group.Participants.Contains(this)) 
            return Error.Validation("User is not a member of the group");
        
        if(group.Admins.Contains(this) && group.Admins.Count == 1)
        {
            return Error.Validation("Cannot leave group as the only admin");
        }
       
        group.Participants.Remove(this);
        AddDomainEvent(new UserLeftGroupEvent(this, group));
        return new Success();
    }
    
    public ErrorOr<Message> SendMessage(string content, Guid? groupId, Guid? recipientId)
    {
        if(groupId != null && Groups.All(g => g.Id != groupId))
        {
            return Error.Validation("User is not a member of the group");
        }
        var message = Message.Create(Id, content, groupId, recipientId);
        if(message.IsError)
        {
            return message;
        }
        AddDomainEvent(new MessagePostedEvent(message.Value));
        return message.Value;
    }
    
    public ErrorOr<Success> AddUsersToGroup(List<User> users, Group group)
    {
        if (!group.Admins.Contains(this))
        {
            return Error.Validation("User is not an admin of the group");
        }
        foreach (var user in users.Where(user => !group.Participants.Contains(user)))
        {
            AddDomainEvent(new UserJoinedGroupEvent(user, group));
            group.Participants.Add(user);
        }
        return new Success();
    }
    
    public ErrorOr<Success> RemoveUsersFromGroup(List<User> users, Group group)
    {
        if (!group.Admins.Contains(this))
        {
            return Error.Validation("User is not an admin of the group");
        }
        foreach (var user in users.Where(user => group.Participants.Contains(user)))
        {
            AddDomainEvent(new UserRemovedFromGroupEvent(this,user, group));
            group.Participants.Remove(user);
        }
        return new Success();
    }
    
    private static bool IsEmailValid(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}