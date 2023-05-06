using Dapper;
using FirstAPI.Extensions;
using FirstAPI.Services;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Data;
using System.Data.SqlClient;

namespace FirstAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("API");
        }

        [HttpPost("/users/register")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var parameters = new
            {
                model.Name,
                model.Email,
                model.Password
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    const string sql = "INSERT INTO [Users] (Name, Email, Password) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Password)";

                    await connection.OpenAsync();

                    int userId = await connection.ExecuteScalarAsync<int>(sql, parameters);

                    var userData = new
                    {
                        Id = userId,
                        model.Name,
                        model.Email
                    };

                    return Ok(new ResultViewModel<dynamic>(userData));
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
                }
            }
        }

        [HttpPost("/users/login")]
        public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var parameters = new
            {
                model.Email
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT * FROM [Users] WHERE Email = @Email";

                await connection.OpenAsync();

                var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

                if (user == null)
                    return StatusCode(401, new ResultViewModel<string>("Usuário não encontrado"));

                if (model.Password != user.Password)
                    return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

                try
                {
                    var token = tokenService.GenerateToken(user);

                    var userData = new
                    {
                        id = user.Id,
                        email = user.Email,
                        token
                    };

                    return Ok(new ResultViewModel<dynamic>(userData, null));
                }
                catch
                {
                    return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));

                }
            }
        }

        [HttpGet("/users")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var parameters = new { UserId = User.GetUserId() };

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

                    var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

                    if (user == null)
                        return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

                    var userData = new
                    {
                        id = user.Id,
                        email = user.Email,
                        name = user.Name
                    };

                    return Ok(new ResultViewModel<dynamic>(userData, null));
                }
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
            }
        }

        [HttpPut("/users")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateUserViewModel model)
        {
            var parameters = new
            {
                UserId = User.GetUserId(),
                model.Name,
                model.Email
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

                var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

                if (user == null)
                    return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

                if (model.Password != user.Password)
                    return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

                try
                {
                    const string query = "UPDATE [Users] SET Name = @Name, Email = @Email WHERE Id = @UserId";

                    await connection.ExecuteAsync(query, parameters);

                    return Ok(new ResultViewModel<string>("Dados editados com sucesso!"));
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
                }
            }
        }

        [HttpDelete("/users")]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            var parameters = new { UserId = User.GetUserId() };

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

                var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

                if (user == null)
                    return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

                try
                {
                    const string query = "DELETE FROM [Users] WHERE Id = @UserId";

                    await connection.ExecuteAsync(query, parameters);

                    return Ok(new ResultViewModel<string>("Conta cancelada com sucesso!"));
                }
                catch
                {
                    return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
                }
            }
        }

        //    var passwordHasher = new PasswordHasher<User>();
        //    var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
        //    if (result != PasswordVerificationResult.Success)
        //        return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));    
    }
}
