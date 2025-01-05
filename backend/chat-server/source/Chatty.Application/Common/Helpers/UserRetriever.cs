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
    private HybridCache _cache;
    
    public UserRetriever(IAppDbContext dbContext, IAuthenticatedUserProvider userProvider, HybridCache cache)
    {
        _dbContext = dbContext;
        _userProvider = userProvider;
        _cache = cache;
    }
    
    // We are getting JWT from auth service. As that is in separate db we need to check if we have
    // user in our db. If not we create one and put it in cache so that we don't check this again.
    public async Task<User?> GetCurrentUser(CancellationToken ct = default){
        var currentUser = _userProvider.GetCurrentUser();
        var validId = Guid.TryParse(currentUser.Id, out var userId);
        if (currentUser.IsGuest || !validId)
        {
            return null;
        }
        
        var user = await _cache.GetOrCreateAsync(CacheKeys.UserIsInDb(currentUser.Id),
            async cancel => await CreateUserInDbIfNotExist(currentUser, cancel),
            cancellationToken: ct);
        
        return user;
    }
    
    private async Task<User> CreateUserInDbIfNotExist(IAuthenticatedUser user, CancellationToken ct)
    {
        var userGuid = Guid.Parse(user.Id);
        var persistedUser = await _dbContext.Set<User>().FirstOrDefaultAsync(x=>x.Id == userGuid);
        if (persistedUser != null)
            return persistedUser;
        
        var newUser = User.Create(user.Email, user.FirstName + " " + user.LastName, userGuid);
        
        await _dbContext.Set<User>().AddAsync(newUser.Value, ct);
        await _dbContext.SaveChangesAsync(ct);
        
        return newUser.Value;
    }
}