using Dapper;
using FirstAPI.Extensions;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Data.SqlClient;

namespace FirstAPI.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly string _connectionString;

        public AccountController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("API");
        }

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var parameters = new
            {
                model.Name,
                model.Email,
                Password = PasswordHasher.Hash(PasswordGenerator.Generate(25))
            };

            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    const string sql = "INSERT INTO [User] (Name, Email, Password) OUTPUT INSERTED.Id VALUES (@Name, @Email, @Password)";

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
                catch (DbUpdateException)
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




        //[HttpPost("v1/accounts/login")]
        //public async Task<IActionResult> Login(
        //[FromBody] LoginViewModel model,
        //[FromServices] DataContext context,
        //[FromServices] TokenService tokenService)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        //    var user = await context
        //        .Users
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(x => x.Email == model.Email);

        //    if (user == null)
        //        return StatusCode(401, new ResultViewModel<string>("Usuário não encontrado"));


        //    var passwordHasher = new PasswordHasher<User>();
        //    var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);


        //    if (result != PasswordVerificationResult.Success)
        //        return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        //    try
        //    {
        //        var token = tokenService.GenerateToken(user);

        //        var userData = new
        //        {
        //            id = user.Id,
        //            email = user.Email,
        //            token
        //        };

        //        return Ok(new ResultViewModel<dynamic>(userData, null));
        //    }
        //    catch
        //    {
        //        return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));

        //    }
        //}

        //[HttpGet("v1/accounts/")]
        //[Authorize]
        //public async Task<IActionResult> Get([FromServices] DataContext context)
        //{
        //    try
        //    {
        //        var user = await context
        //            .Users
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

        //        if (user == null)
        //            return StatusCode(401, new ResultViewModel<string>("Usuário não encontrado"));

        //        var userData = new
        //        {
        //            id = user.Id,
        //            email = user.Email,
        //            name = user.Name
        //        };

        //        return Ok(new ResultViewModel<dynamic>(userData, null));
        //    }
        //    catch
        //    {
        //        return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
        //    }
        //}

        //[HttpPut("v1/accounts")]
        //[Authorize]
        //public async Task<IActionResult> Put(
        //    [FromBody] UpdateUserViewModel model,
        //    [FromServices] DataContext context)
        //{
        //    try
        //    {
        //        var user = await context
        //            .Users
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

        //        if (user == null)
        //            return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

        //        var passwordHasher = new PasswordHasher<User>();
        //        var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        //        if (result != PasswordVerificationResult.Success)
        //            return StatusCode(401, new ResultViewModel<string>("Senha inválida"));

        //        user.Name = model.Name;
        //        user.Email = model.Email;

        //        context.Users.Update(user);
        //        await context.SaveChangesAsync();

        //        var userData = new
        //        {
        //            id = user.Id,
        //            email = user.Email,
        //            name = user.Name
        //        };

        //        return Ok(new ResultViewModel<string>("Dados editados com sucesso!"));
        //    }
        //    catch (DbUpdateException)
        //    {
        //        return StatusCode(400, new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
        //    }
        //}

        //[HttpDelete("v1/accounts/")]
        //[Authorize]
        //public async Task<IActionResult> Delete([FromServices] DataContext context)
        //{
        //    try
        //    {
        //        var user = await context
        //            .Users
        //            .AsNoTracking()
        //            .FirstOrDefaultAsync(x => x.Id == User.GetUserId());

        //        if (user == null)
        //            return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

        //        context.Users.Remove(user);
        //        await context.SaveChangesAsync();

        //        return Ok(new ResultViewModel<string>("Conta cancelada com sucesso!"));
        //    }
        //    catch
        //    {
        //        return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
        //    }
        //}

    }
}
