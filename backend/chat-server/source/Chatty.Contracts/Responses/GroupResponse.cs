namespace Chatty.Contracts.Responses;

public sealed record GroupResponse(
    Guid Id,
    string Name,
    bool IsPublic,
    bool IsAdmin = false);