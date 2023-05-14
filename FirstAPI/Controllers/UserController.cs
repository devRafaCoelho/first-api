using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.Repositories.Contracts;
using FirstAPI.Services;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureIdentity.Password;
using System.Data.SqlClient;

namespace FirstAPI.Controllers;

[ApiController]

public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
        => _userRepository = userRepository;


    [HttpPost("/users/register")]
    [ProducesResponseType(typeof(UserResult), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.AddUserAsync(model);

            var result = new UserResult
            {
                Id = user.Id,
                Name = model.Name,
                Email = model.Email
            };

            return Ok(result);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return BadRequest(new { error = "E-mail já cadastrado." });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpPost("/users/login")]
    [ProducesResponseType(typeof(LoginResult), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> LoginAsync(
    [FromBody] LoginViewModel model,
    [FromServices] TokenService tokenService)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
                return NotFound(new { error = "Usuário não encontrado" });

            if (!PasswordHasher.Verify(user.Password ?? "", model.Password))
                return Unauthorized(new { error = "Usuário ou senha incorretos." });

            var token = tokenService.GenerateToken(user);

            var result = new LoginResult
            {
                Id = user.Id,
                Email = user.Email,
                Token = token
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { Message = "Falha interna no servidor" });
        }
    }

    [HttpGet("/users")]
    [Authorize]
    [ProducesResponseType(typeof(UserResult), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> GetUserData()
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByIdAsync(User.GetUserId());

            if (user == null)
                return NotFound(new { error = "Usuário não encontrado" });

            var result = new UserResult
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpPut("/users")]
    [Authorize]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUserViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByIdAsync(User.GetUserId());

            if (user == null)
                return NotFound(new { error = "Usuário não encontrado" });

            if (!PasswordHasher.Verify(user.Password ?? "", model.Password))
                return Unauthorized(new { error = "Senha incorreta." });


            await _userRepository.UpdateUserByIdAsync(model, User.GetUserId());

            return Ok(new { Message = "Dados editados com sucesso!" });

        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return BadRequest(new { error = "E-mail já cadastrado." });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpDelete("/users")]
    [Authorize]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]
    public async Task<IActionResult> Delete()
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByIdAsync(User.GetUserId());

            if (user == null)
                return NotFound(new { error = "Usuário não encontrado" });

            await _userRepository.DeleteUserByIdAsync(User.GetUserId());

            return Ok(new { Message = "Conta cancelada com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }
}







