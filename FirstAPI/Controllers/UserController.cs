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

    [HttpPost("/user/register")]
    [ProducesResponseType(typeof(UserViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> RegisterUserAsync([FromBody] User model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var userEmail = await _userRepository.GetUserByEmailAsync(model.Email);

            if (userEmail != null)
                return BadRequest(new ErrorViewModel("email", "E-mail já cadastrado."));

            var userCPF = await _userRepository.GetUserByCpfAsync(model.CPF);

            if (userCPF != null)
                return BadRequest(new ErrorViewModel("cpf", "CPF já cadastrado."));

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new ErrorViewModel("password", "As senhas não coincidem."));

            var user = await _userRepository.AddUserAsync(model);

            var result = new UserViewModel
            {
                Id = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CPF = model?.CPF,
                Phone = model?.Phone
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new ErrorViewModel("global", "Falha interna no servidor"));
        }
    }

    [HttpPost("/user/login")]
    [ProducesResponseType(typeof(LoginViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> LoginAsync([FromBody] Login model, [FromServices] TokenService tokenService)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
                return Unauthorized(new ErrorViewModel("email", "Email incorreto."));

            if (!PasswordHasher.Verify(user.Password ?? "", model.Password))
                return Unauthorized(new ErrorViewModel("password", "Senha incorreta."));

            var token = tokenService.GenerateToken(user);

            var result = new LoginViewModel
            {
                User = new UserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    CPF = user.CPF,
                    Phone = user.Phone
                },
                Token = token
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new ErrorViewModel("global", "Falha interna no servidor"));
        }
    }

    [HttpGet("/user")]
    [Authorize]
    [ProducesResponseType(typeof(UserViewModel), 200)]
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
                return NotFound(new ErrorViewModel("data", "Nenhum usuário encontrado."));

            var result = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CPF = user.CPF,
                Phone = user.Phone
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new ErrorViewModel("global", "Falha interna no servidor"));
        }
    }

    [HttpPut("/user")]
    [Authorize]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateUser model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var user = await _userRepository.GetUserByIdAsync(User.GetUserId());

            if (user == null)
                return NotFound(new ErrorViewModel("data", "Nenhum usuário encontrado."));

            if (!PasswordHasher.Verify(user.Password ?? "", model.Password ?? ""))
                return Unauthorized(new ErrorViewModel("password", "Senha incorreta."));

            var hasUserCpf = await _userRepository.GetUserByCpfAsync(model.CPF ?? "");

            if (hasUserCpf != null)
                return BadRequest(new ErrorViewModel("cpf", "CPF já cadastrado."));

            await _userRepository.UpdateUserByIdAsync(model, User.GetUserId());

            return Ok(new { Message = "Dados atualizados com sucesso!" });
        }
        catch (SqlException)
        {
            return BadRequest(new ErrorViewModel("email", "E-mail já cadastrado."));
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpDelete("/user")]
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
                return NotFound(new ErrorViewModel("data", "Nenhum usuário encontrado."));

            await _userRepository.DeleteUserByIdAsync(User.GetUserId());

            return Ok(new { Message = "Conta cancelada com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }
}







