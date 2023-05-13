using Dapper;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using SecureIdentity.Password;
using System.Data;
using System.Data.SqlClient;

namespace FirstAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string? _connectionString;

    public UserRepository(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString("API");

    public async Task<User> AddUserAsync(RegisterUserViewModel model)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "INSERT INTO [Users] (Name, Email, Password) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Password)";

            var parameters = new
            {
                model.Name,
                model.Email,
                Password = PasswordHasher.Hash(model.Password ?? "")
            };

            var result = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

            return result;
        }
    }
}

