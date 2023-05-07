using Dapper;
using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace FirstAPI.Controllers
{
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly string _connectionString;

        public ProductsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("API");
        }

        [HttpPost("/products/register")]
        [Authorize]
        public async Task<IActionResult> PostAsync([FromBody] RegisterProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

                var parameters = new
                {
                    model.Name,
                    model.Description,
                    User_Id = User.GetUserId()
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql = "INSERT INTO [Products] (Name, Description, User_Id) OUTPUT INSERTED.Id VALUES (@Name, @Description, @User_Id)";

                    await connection.OpenAsync();

                    int productId = await connection.ExecuteScalarAsync<int>(sql, parameters);

                    var productData = new
                    {
                        Id = productId,
                        model.Name,
                        model.Description,
                        User_Id = User.GetUserId()
                    };

                    return Ok(new ResultViewModel<dynamic>(productData));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
            }
        }

        [HttpGet("/products")]
        [Authorize]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 1)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

                var parameters = new
                {
                    User_Id = User.GetUserId(),
                    Skip = page * pageSize,
                    Take = pageSize
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string countSql = "SELECT COUNT(*) AS Total FROM [Products] WHERE User_Id = @User_Id";
                    var count = await connection.ExecuteScalarAsync<int>(countSql, parameters);

                    const string sql = "SELECT * FROM [Products] WHERE User_Id = @User_Id ORDER BY Id OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY";
                    var products = await connection.QueryAsync<Product>(sql, parameters);

                    return Ok(new ResultViewModel<dynamic>(new
                    {
                        total = count,
                        page,
                        pageSize,
                        products
                    }));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
            }
        }


        [HttpGet("/products/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

                var parameters = new
                {
                    id,
                    User_Id = User.GetUserId()
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM [Products] WHERE Id = @id AND User_Id = @User_Id";

                    var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);

                    if (product == null)
                        return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

                    return Ok(new ResultViewModel<Product>(product));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
            }
        }

        [HttpPut("/products/{id}")]
        [Authorize]
        public async Task<IActionResult> Put(
            [FromRoute] int id,
            [FromBody] Product model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

                var parameters = new
                {
                    id,
                    model.Name,
                    model.Description,
                    User_Id = User.GetUserId()
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM [Products] WHERE Id = @id AND User_Id = @User_Id";

                    var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);

                    if (product == null)
                        return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

                    const string query = "UPDATE [Products] SET Name = @Name, Description = @Description WHERE Id = @id";

                    await connection.ExecuteAsync(query, parameters);

                    return Ok(new ResultViewModel<string>("Produto editado com sucesso."));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
            }
        }

        [HttpDelete("/products/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

                var parameters = new
                {
                    id,
                    User_Id = User.GetUserId()
                };

                using (var connection = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM [Products] WHERE Id = @id AND User_Id = @User_Id";

                    var product = await connection.QueryFirstOrDefaultAsync<Product>(sql, parameters);

                    if (product == null)
                        return NotFound(new ResultViewModel<Product>("Produto não encontrado"));

                    const string query = "DELETE FROM [Products] WHERE Id = @id;";

                    await connection.ExecuteAsync(query, parameters);

                    return Ok(new ResultViewModel<string>("Produto removido com sucesso"));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Product>("Falha interna no servidor"));
            }
        }
    }
}

