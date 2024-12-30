using Chatty.Application.Common.Interfaces;
using Chatty.Application.Common.Repositories;
using Chatty.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chatty.Application.Common.Helpers;

public class UserRetriever : IUserRetriever
{
    private readonly IChattyDbContext _dbContext;
    private readonly IUserSession  _userSession;
    private User? _currentUser = null;
    
    public UserRetriever(IChattyDbContext dbContext, IUserSession userSession)
    {
        _dbContext = dbContext;
        _userSession = userSession;
    }
    
    public async Task<User?> GetCurrentUser(){
        var currentUser = _userSession.GetCurrentUser();
        if (_currentUser != null && _currentUser.Id == currentUser.Id)
        {
            return _currentUser;
        }
        _currentUser = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == currentUser.Id);
        return _currentUser;
    }
}