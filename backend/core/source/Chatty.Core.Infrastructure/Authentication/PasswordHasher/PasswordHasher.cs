using System.Text.RegularExpressions;
using Chatty.Core.Domain;
using ErrorOr;

namespace Chatty.Core.Infrastructure.Authentication.PasswordHasher;

public partial class PasswordHasher : IPasswordHasher
{
    private static readonly Regex PasswordRegex = HardPasswordRegex();

    public ErrorOr<string> HashPassword(string password)
    {
        return !PasswordRegex.IsMatch(password)
            ? Error.Validation(description: "Password too weak")
            : BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool IsCorrectPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }

    // https://stackoverflow.com/a/34715674/10091553
    [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled)]
    private static partial Regex HardPasswordRegex();
}