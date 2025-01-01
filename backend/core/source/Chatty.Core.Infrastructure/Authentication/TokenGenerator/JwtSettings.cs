namespace Chatty.Core.Infrastructure.Authentication.TokenGenerator;

public class JwtSettings
{
    public const string Section = "JwtSettings";

    public string Audience { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Secret { get; set; } = null!;
    public int TokenExpirationInMinutes { get; set; } = 10;
    public int RefreshTokenExpirationInHours { get; set; } = 24;
}