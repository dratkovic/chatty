using System.Net.Http.Headers;
using Bogus;

namespace Chatty.WebApi.Tests.Integration;

public static class Extensions
{
    // Bogus doesn't support records, so we need to create a custom extension method to support records
    public static Faker<T> WithRecord<T>(this Faker<T> faker) where T : class
    {
        faker.CustomInstantiator(_ => (System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(T)) as T)!);
        return faker;
    }

    public static HttpClient AddAuthorizationToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static HttpClient RemoveAuthorizationToken(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
        return client;
    }
}
