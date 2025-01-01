using Chatty.Application.Common.Models;

namespace Chatty.Application.Common.Interfaces;

public interface IUserSession
{
    CurrentUser GetCurrentUser();
}