using Chatty.Core.Application.Common.Persistance;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Application.Common.Helpers;

public class UserRetriever : IUserRetriever
{
    private readonly IAppDbContext _dbContext;
    private readonly IUserRetriever  _userSession;
    private User? _currentUser = null;
    
    public UserRetriever(IAppDbContext dbContext, IUserRetriever userSession)
    {
        _dbContext = dbContext;
        _userSession = userSession;
    }
    
    public async Task<User?> GetCurrentUser(){
        var currentUser = await _userSession.GetCurrentUser();
        if (_currentUser != null && _currentUser.Id == currentUser.Id)
        {
            return _currentUser;
        }
        _currentUser = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == currentUser.Id);
        return _currentUser;
    }
}