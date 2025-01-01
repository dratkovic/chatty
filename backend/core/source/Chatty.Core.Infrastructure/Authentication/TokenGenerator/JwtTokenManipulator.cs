using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using ErrorOr;

namespace Chatty.Core.Infrastructure.Authentication.TokenGenerator;

public class JwtTokenManipulator : IJwtTokenManipulator
{
    private readonly JwtSettings _jwtSettings;
    public  int TokenExpirationInMinutes { get; init; }
    public  int RefreshTokenExpirationInHours { get; init; }

    public JwtTokenManipulator(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
        TokenExpirationInMinutes = _jwtSettings.TokenExpirationInMinutes;
        RefreshTokenExpirationInHours = _jwtSettings.RefreshTokenExpirationInHours;
    }

    public string GenerateToken(IAuthenticationUser user)
    {
        var tokenHandler = new JsonWebTokenHandler();
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };
        
        claims.AddRange(user.Roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        // Create the token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes), // Token expiry time
            Issuer =  _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        
        using var rng = RandomNumberGenerator.Create();
        
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    public async Task<ErrorOr<ClaimsIdentity>> GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // Ignore expiration
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))
        };
        
        
        var tokenHandler = new JsonWebTokenHandler();
        
        var principal = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
        
        if(principal is null)
            return Error.Validation("token", "Invalid token");
        
        if (principal.SecurityToken is not JsonWebToken jwtToken || !jwtToken.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return Error.Validation("token", "Invalid token");

        return principal.ClaimsIdentity;
    }
}