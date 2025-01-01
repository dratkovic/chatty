using System.Security.Claims;
using ErrorOr;
using Sensei.Core.Domain.Models;

namespace Sensei.Core.Application.Common.Interfaces;

public interface IJwtTokenManipulator
{
    string GenerateToken(IAuthenticationUser user);
    string GenerateRefreshToken();
    Task<ErrorOr<ClaimsIdentity>> GetPrincipalFromExpiredToken(string token);
    int TokenExpirationInMinutes { get; init; }
    int RefreshTokenExpirationInHours { get; init; }
}