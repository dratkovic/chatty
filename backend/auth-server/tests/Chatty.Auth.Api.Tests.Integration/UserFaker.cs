using Bogus;
using Chatty.Authentication.Api.Features.User.Register;

namespace Chatty.Auth.Api.Tests.Integration;

public static class UserFaker
{
    public static Faker<RegisterRequest> GetRegisterFaker()
    {
        return new Faker<RegisterRequest>()
        .WithRecord()
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.Password, f => "P@ssword12!")
            .RuleFor(x => x.FirstName, x => x.Name.FirstName())
            .RuleFor(x => x.LastName, x => x.Name.LastName());
    }
}
