namespace Chatty.Contracts.Requests;

public sealed record UsersGroupRequest(List<Guid> Users, Guid GroupId);