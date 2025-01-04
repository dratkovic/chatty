namespace Chatty.Application.Common;

internal static  class CacheKeys
{
    internal static string GroupUsers(Guid groupId)
    {
        return $"GroupUsers:{groupId}";
    }
}