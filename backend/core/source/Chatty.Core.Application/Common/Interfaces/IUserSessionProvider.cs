using Chatty.Core.Application.Common.Models;

namespace Chatty.Core.Application.Common.Interfaces;

public interface IUserSessionProvider
{
    Task<UserSession> GetUserSessionAsync();
}