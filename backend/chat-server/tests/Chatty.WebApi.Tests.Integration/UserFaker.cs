using Bogus;
using Chatty.Core.Application.Common.Models;
using Chatty.Core.Domain.Models;

namespace Chatty.Auth.Api.Tests.Integration;

public static class UserFaker
{
    public static IAuthenticatedUser GetAuthenticatedUser()
    {
        var faker = new Faker();
        return new AuthenticatedUser(faker.Random.Guid(), faker.Internet.Email(), faker.Name.FirstName(), faker.Name.LastName(), []);
    }
}
