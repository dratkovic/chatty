using Chatty.Core.Domain.Models;

namespace Chatty.Core.Application.Common.Interfaces;

public interface IAuthenticatedUserProvider
{
    IAuthenticationUser GetCurrentUser();
}