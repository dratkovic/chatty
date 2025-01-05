using Chatty.Domain;

namespace Chatty.Application.Common.Helpers;

public interface IUserRetriever
{
    Task<User?> GetCurrentUser(CancellationToken ct = default);
}