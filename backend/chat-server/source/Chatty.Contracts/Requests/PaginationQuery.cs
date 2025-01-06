namespace Chatty.Contracts.Requests;

public class MessagesQuery
{
    public int Count { get; set; } = 20;
    public Guid? FriendId { get; set; }
    public Guid? GroupId { get; set; }
    public DateTime? OlderThan { get; set; }
}
