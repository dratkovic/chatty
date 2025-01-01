using System.Security.Claims;
using Chatty.Core.Domain.Models;
using ErrorOr;

namespace Chatty.Core.Application.Common.Interfaces;

public interface IJwtTokenManipulator
{
    string GenerateToken(IAuthenticationUser user);
    string GenerateRefreshToken();
    Task<ErrorOr<ClaimsIdentity>> GetPrincipalFromExpiredToken(string token);
    int TokenExpirationInMinutes { get; init; }
    int RefreshTokenExpirationInHours { get; init; }
}