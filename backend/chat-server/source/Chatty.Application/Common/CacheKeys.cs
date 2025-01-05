namespace Chatty.Application.Common;

internal static  class CacheKeys
{
    internal static string GroupUsers(Guid groupId)
    {
        return $"GroupUsers:{groupId}";
    }
    
    internal static string UserIsInDb(string userId)
    {
        return $"UserSession:{userId}";
    }
}