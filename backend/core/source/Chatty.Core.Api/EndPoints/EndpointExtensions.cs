using System.Reflection;

namespace Chatty.Core.Api.EndPoints;

public static class EndpointExtensions
{
    public static void UseIEndpoints<TMarker>(this IApplicationBuilder app)
    {
        UseIEndpoints(app, typeof(TMarker));
    }

    public static void UseIEndpoints(this IApplicationBuilder app, Type typeMarker)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoint.DefineEndpoint))!
                .Invoke(null, [app]);
        }
    }

    private static IEnumerable<TypeInfo> GetEndpointTypesFromAssemblyContaining(Type typeMarker)
    {
        var endpointTypes = typeMarker.Assembly.DefinedTypes
            .Where(x => x is { IsAbstract: false, IsInterface: false } &&
                        typeof(IEndpoint).IsAssignableFrom(x));
        return endpointTypes;
    }
}
