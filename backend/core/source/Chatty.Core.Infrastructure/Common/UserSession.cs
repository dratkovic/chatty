﻿using System.Security.Claims;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Models;
using Chatty.Core.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Chatty.Core.Infrastructure.Common;

public class AuthenticatedUserProvider(IHttpContextAccessor? _httpContextAccessor) : IAuthenticatedUserProvider
{
    public virtual IAuthenticationUser GetCurrentUser()
    {
        if (_httpContextAccessor?.HttpContext is null)
            return AuthenticationUser.GuestUser;

        if (!_httpContextAccessor.HttpContext!.User.Claims.Any())
            return AuthenticationUser.GuestUser;

        var id = GetClaimValues(ClaimTypes.NameIdentifier)
            .First();

        var email = GetClaimValues(ClaimTypes.Email).First();
        var firstName = GetClaimValues(ClaimTypes.Name).First();
        var lastName = GetClaimValues(ClaimTypes.Surname).First();
        var roles = GetClaimValues(ClaimTypes.Role).ToList();

        return new AuthenticationUser(id, email, firstName, lastName, roles);
    }

    protected IReadOnlyList<string> GetClaimValues(string claimType)
    {
        return _httpContextAccessor!.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();
    }
}
