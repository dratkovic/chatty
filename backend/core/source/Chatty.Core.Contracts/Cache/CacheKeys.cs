namespace Chatty.Core.Contracts.Cache;

public static class CacheKeys
{
    public static string UserSession(string userId) => $"UserSession:{userId}";
}