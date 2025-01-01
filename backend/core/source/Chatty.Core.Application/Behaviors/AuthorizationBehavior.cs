using System.Reflection;
using Chatty.Core.Application.Common.Authorization;
using Chatty.Core.Application.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Chatty.Core.Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(IAuthenticatedUserProvider authenticatedUserProvider)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // by default user must be authenticated to take any action
        // only if there is AllowGuest attribute on the request, then guest user can take the action

        var allowGuest = request.GetType()
            .GetCustomAttributes<AllowGuests>()
            .ToList();
        if (allowGuest.Count > 0)
        {
            return await next();
        }

        var currentUser = authenticatedUserProvider.GetCurrentUser();

        if (currentUser.IsGuest)
        {
            return (dynamic)Error.Unauthorized(description: "User is forbidden from taking this action");
        }

        return await next();

        // ****  in future if needed we can implement some additional authorization logic here  ****

        // var authorizationAttributes = request.GetType()
        //     .GetCustomAttributes<SAuthorizeAttribute>()
        //     .ToList();
        //
        // if (authorizationAttributes.Count == 0)
        // {
        //     return await next();
        // }
        //
        // return await next();
    }
}