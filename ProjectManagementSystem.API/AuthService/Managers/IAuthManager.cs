using ProjectManagementSystem.Common.Contracts.Auth;

namespace ProjectManagementSystem.API.AuthService.Managers;

public interface IAuthManager
{
    Task<string> SignInAsync(string username, string password);
}