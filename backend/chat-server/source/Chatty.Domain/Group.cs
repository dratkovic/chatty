using Chatty.Domain.Common;

namespace Chatty.Domain;

public sealed class Group: AuditEntityBase
{
    public bool IsPublic { get; set; }
    public string Name { get; private set; }
    public List<User> Admins { get;} = [];
    public List<User> Participants { get; } = [];
    
    internal Group(User creator, string name, bool isPulbic, Guid? id = null) : base(id)
    {
        Name = name;
        IsPublic = isPulbic;
        Participants.Add(creator);
        Admins.Add(creator);
    }
}