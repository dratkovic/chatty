namespace Sensei.Core.Application.Common.Cache;

public interface ICacheService
{
    Task InvalidateCache(string key, CancellationToken cancellationToken);
}
