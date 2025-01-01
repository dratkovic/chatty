using Microsoft.Extensions.Logging;

namespace Chatty.Application.Common.Helpers;

public static class LoggerExtensions
{
    public static void LogUnauthenticatedUserIsTryingTo<T>(this ILogger<T> logger, string action)
    {
        logger.LogWarning("Unauthenticated User is trying to: {0}", action);
    }
}