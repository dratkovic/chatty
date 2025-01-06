using System.Collections.Concurrent;
using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Core.Domain.Models;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace Chatty.Application.Common.Helpers;

public class UserRetriever : IUserRetriever
{
    private readonly IAppDbContext _dbContext;
    private readonly IAuthenticatedUserProvider _userProvider;
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public UserRetriever(IAppDbContext dbContext, IAuthenticatedUserProvider userProvider)
    {
        _dbContext = dbContext;
        _userProvider = userProvider;
    }

    // We are getting JWT from auth service. As that is in separate db we need to check if we have
    // user in our db. If not we create one and put it in cache so that we don't check this again.
    public async Task<User?> GetCurrentUser(CancellationToken ct = default)
    {
        var currentUser = _userProvider.GetCurrentUser();
        if (currentUser.IsGuest)
        {
            return null;
        }

        if (_users.TryGetValue(currentUser.Id, out var user))
        {
            return user;
        }

        var persistedUser = await CreateUserInDbIfNotExist(currentUser, ct);
        _users.TryAdd(currentUser.Id, persistedUser);

        return persistedUser;
    }

    private async Task<User> CreateUserInDbIfNotExist(IAuthenticatedUser user, CancellationToken ct)
    {
        var persistedUser = await _dbContext.Set<User>().FirstOrDefaultAsync(x => x.Id == user.Id);
        if (persistedUser != null)
            return persistedUser;

        var newUser = User.Create(user.Email, user.FirstName + " " + user.LastName, user.Id);

        await _dbContext.Set<User>().AddAsync(newUser.Value, ct);
        await _dbContext.SaveChangesAsync(ct);

        return newUser.Value;
    }
}