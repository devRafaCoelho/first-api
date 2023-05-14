using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IUserRepository
{
    Task<User> AddUserAsync(UserViewModel user);

    Task<User> GetUserByEmailAsync(string email);

    Task<User> GetUserByIdAsync(int id);

    Task<User> UpdateUserByIdAsync(UserViewModel user, int id);

    Task<User> DeleteUserByIdAsync(int id);
}

