using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Interfaces;

public interface IAuthenticatedUserProvider
{
    IAuthenticationUser GetCurrentUser();
}