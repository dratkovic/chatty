using Chatty.Core.Application.Common.Interfaces;
using Chatty.Core.Application.Common.Persistance;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Application.Common.Helpers;

public class UserRetriever : IUserRetriever
{
    private readonly IAppDbContext _dbContext;
    private readonly IAuthenticatedUserProvider _userProvider;
    private User? _currentUser = null;
    
    public UserRetriever(IAppDbContext dbContext, IAuthenticatedUserProvider userProvider)
    {
        _dbContext = dbContext;
        _userProvider = userProvider;
    }
    
    public async Task<User?> GetCurrentUser(){
        var currentUser = _userProvider.GetCurrentUser();
        var validId = Guid.TryParse(currentUser.Id, out var userId);
        if (currentUser.IsGuest || !validId)
        {
            return null;
        }
        
        if (_currentUser != null && _currentUser.Id == userId)
        {
            return _currentUser;
        }
        
        _currentUser = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == userId);
        return _currentUser;
    }
}