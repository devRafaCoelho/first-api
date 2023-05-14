using Dapper;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using System.Data;
using System.Data.SqlClient;

namespace FirstAPI.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string? _connectionString;

    public ProductRepository(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString("API");

    public async Task<Product> AddProductAsync(ProductViewModel model, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "INSERT INTO [Products] (Name, Description, UserId) OUTPUT INSERTED.Id VALUES (@Name, @Description, @id)";

            var parameters = new
            {
                model.Name,
                model.Description,
                id
            };

            var result = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);

            return result;
        }
    }

    public async Task<int> GetCountProductsByUserId(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT COUNT(*) AS Total FROM [Products] WHERE UserId = @id";

            var parameters = new { id };

            var result = await connection.QueryFirstOrDefaultAsync<int>(sql, parameters);

            return result;
        }
    }

    public async Task<List<Product>> GetAllProductsByUserId(int skip, int take, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT * FROM [Products] WHERE UserId = @id ORDER BY Id OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY";

            var parameters = new
            {
                skip,
                take,
                id
            };

            var result = await connection.QueryAsync<Product>(sql, parameters);

            return result.ToList();
        }
    }

    public async Task<Product> GetProductById(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "SELECT * FROM [Products] WHERE Id = @id";

            var parameters = new { id };

            var result = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);

            return result;
        }
    }
    public async Task<Product> UpdateProductByIdAsync(ProductViewModel model, int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "UPDATE [Products] SET Name = @Name, Description = @Description WHERE Id = @id";

            var parameters = new
            {
                model.Name,
                model.Description,
                id
            };

            await connection.ExecuteAsync(sql, parameters);

            return await GetProductById(id);
        }
    }

    public async Task<Product> DeleteProductByIdAsync(int id)
    {
        using (IDbConnection connection = new SqlConnection(_connectionString))
        {
            const string sql = "DELETE FROM [Products] WHERE Id = @id;";

            var parameters = new { id };

            await connection.ExecuteAsync(sql, parameters);

            return await GetProductById(id);
        }
    }
}

