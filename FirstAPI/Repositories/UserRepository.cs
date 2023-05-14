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

    public async Task<User> GetUserByEmailAsync(string email)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT * FROM [Users] WHERE Email = @email";

            var parameters = new { email };

            var result = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

            return result;
        }
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT * FROM [Users] WHERE Id = @id";

            var parameters = new { id };

            var result = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

            return result;
        }
    }

    public async Task<User> UpdateUserByIdAsync(UpdateUserViewModel model, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "UPDATE [Users] SET Name = @Name, Email = @Email WHERE Id = @id";

            var parameters = new
            {
                id,
                model.Name,
                model.Email
            };

            await connection.ExecuteAsync(sql, parameters);

            return await GetUserByIdAsync(id);
        }
    }

    public async Task<User> DeleteUserByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "DELETE FROM [Users] WHERE Id = @id";

            var parameters = new { id };

            await connection.ExecuteAsync(sql, parameters);

            return await GetUserByIdAsync(id);
        }
    }
}

