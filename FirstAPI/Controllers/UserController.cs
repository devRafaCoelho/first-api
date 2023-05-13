using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace FirstAPI.Controllers;

[ApiController]

public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
        => _userRepository = userRepository;


    [HttpPost("/users/register")]
    [ProducesResponseType(typeof(User), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> Post([FromBody] RegisterUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.AddUserAsync(model);

            var userdata = new
            {
                id = user.Id,
                name = model.Name,
                email = model.Email
            };

            return Ok(userdata);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return BadRequest(new { error = "E-mail já está cadastrado." });
        }
        catch
        {
            return StatusCode(500, new { Mensagem = "Falha interna no servidor" });
        }
    }
}

//[HttpPost("/users/login")]
//public async Task<IActionResult> Login(
//[FromBody] LoginViewModel model,
//[FromServices] TokenService tokenService)
//{
//    try
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//        var parameters = new
//        {
//            model.Email
//        };

//        using (var connection = new SqlConnection(_connectionString))
//        {
//            const string sql = "SELECT * FROM [Users] WHERE Email = @Email";

//            await connection.OpenAsync();

//            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

//            if (user == null)
//                return StatusCode(401, new ResultViewModel<string>("Usuário não encontrado"));

//            if (!PasswordHasher.Verify(user.Password, model.Password))
//                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha incorretos."));


//            var token = tokenService.GenerateToken(user);

//            var userData = new
//            {
//                id = user.Id,
//                email = user.Email,
//                token
//            };

//            return Ok(new ResultViewModel<dynamic>(userData, null));
//        }
//    }
//    catch
//    {
//        return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));

//    }
//}

//[HttpGet("/users")]
//[Authorize]
//public async Task<IActionResult> Get()
//{
//    try
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//        var parameters = new { UserId = User.GetUserId() };

//        using (var connection = new SqlConnection(_connectionString))
//        {
//            await connection.OpenAsync();

//            const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

//            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

//            if (user == null)
//                return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

//            var userData = new
//            {
//                id = user.Id,
//                email = user.Email,
//                name = user.Name
//            };

//            return Ok(new ResultViewModel<dynamic>(userData, null));
//        }
//    }
//    catch
//    {
//        return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
//    }
//}

//[HttpPut("/users")]
//[Authorize]
//public async Task<IActionResult> Put([FromBody] UpdateUserViewModel model)
//{
//    try
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//        var parameters = new
//        {
//            UserId = User.GetUserId(),
//            model.Name,
//            model.Email
//        };

//        using (var connection = new SqlConnection(_connectionString))
//        {
//            await connection.OpenAsync();

//            const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

//            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

//            if (user == null)
//                return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

//            if (!PasswordHasher.Verify(user.Password, model.Password))
//                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha incorretos."));


//            const string query = "UPDATE [Users] SET Name = @Name, Email = @Email WHERE Id = @UserId";

//            await connection.ExecuteAsync(query, parameters);

//            return Ok(new ResultViewModel<string>("Dados editados com sucesso!"));
//        }
//    }
//    catch (SqlException ex) when (ex.Number == 2627)
//    {
//        return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.Message);
//        return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
//    }

//}

//[HttpDelete("/users")]
//[Authorize]
//public async Task<IActionResult> Delete()
//{
//    try
//    {
//        if (!ModelState.IsValid)
//            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

//        var parameters = new { UserId = User.GetUserId() };

//        using (var connection = new SqlConnection(_connectionString))
//        {
//            await connection.OpenAsync();

//            const string sql = "SELECT * FROM [Users] WHERE Id = @UserId";

//            var user = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters);

//            if (user == null)
//                return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

//            const string query = "DELETE FROM [Users] WHERE Id = @UserId";

//            await connection.ExecuteAsync(query, parameters);

//            return Ok(new ResultViewModel<string>("Conta cancelada com sucesso!"));
//        }
//    }
//    catch
//    {
//        return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
//    }
//}