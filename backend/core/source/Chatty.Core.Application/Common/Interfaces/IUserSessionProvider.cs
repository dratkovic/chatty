using Sensei.Core.Application.Common.Models;

namespace Sensei.Core.Application.Common.Interfaces;

public interface IUserSessionProvider
{
    Task<UserSession> GetUserSessionAsync();
}