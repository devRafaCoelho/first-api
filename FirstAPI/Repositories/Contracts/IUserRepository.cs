using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IUserRepository
{
    Task<User> AddUserAsync(RegisterUserViewModel user);
}

