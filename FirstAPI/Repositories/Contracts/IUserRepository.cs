using FirstAPI.Models;
using FirstAPI.ViewModels;

namespace FirstAPI.Repositories.Contracts;

public interface IUserRepository
{
    Task<User> AddUserAsync(User model);

    Task<User> GetUserByEmailAsync(string email);

    Task<User> GetUserByIdAsync(int id);

    Task<User> GetUserByCpfAsync(string cpf);

    Task<User> UpdateUserByIdAsync(UpdateUser model, int id);

    Task<User> DeleteUserByIdAsync(int id);
}

