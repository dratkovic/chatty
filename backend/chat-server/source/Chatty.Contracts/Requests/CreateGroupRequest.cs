namespace Chatty.Contracts.Requests;

public sealed record CreateGroupRequest(string Name, bool IsPublic);