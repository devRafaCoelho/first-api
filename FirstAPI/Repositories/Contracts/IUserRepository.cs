using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IUserRepository
{
    Task<User> AddUserAsync(RegisterUserViewModel user);

    Task<User> GetUserByEmailAsync(string email);

    Task<User> GetUserByIdAsync(int id);

    Task<User> UpdateUserByIdAsync(UpdateUserViewModel user, int id);

    Task<User> DeleteUserByIdAsync(int id);
}

