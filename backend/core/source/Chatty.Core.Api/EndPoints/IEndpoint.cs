namespace Sensei.Core.Api.EndPoints;

public interface IEndpoint
{
    public static abstract void DefineEndpoint(IEndpointRouteBuilder app);
}
