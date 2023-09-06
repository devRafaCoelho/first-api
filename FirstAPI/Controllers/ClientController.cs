using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace FirstAPI.Controllers;

[ApiController]
[Authorize]

public class ClientController : ControllerBase
{
    private readonly IClientRepository _clientRepository;

    public ClientController(IClientRepository clientRepository)
        => _clientRepository = clientRepository;


    [HttpPost("/client/register")]
    [ProducesResponseType(typeof(Client), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> RegisterClientAsync([FromBody] Client model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            //var userEmail = await _userRepository.GetUserByEmailAsync(model.Email);

            //if (userEmail != null)
            //    return BadRequest(new ErrorViewModel("email", "E-mail já cadastrado."));

            var client = await _clientRepository.AddClientAsync(model);

            var result = new ClientViewModel
            {
                Id = client.Id,
                Name = model.Name,
                Email = model.Email,
                CPF = model.CPF,
                Phone = model.Phone,
                Address = model.Address,
                Complement = model.Complement,
                Zip_Code = model.Zip_Code,
                District = model.District,
                City = model.City,
                UF = model.UF,
            };

            return Ok(result);
        }
        catch (SqlException)
        {
            return BadRequest(new { error = "E-mail ou CPF já cadastrado." });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpGet("/client")]
    [ProducesResponseType(typeof(List<ClientViewModel>), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> ListClientsAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 25)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var count = await _clientRepository.GetCountClientsAsync();

            if (count == 0)
                return NotFound(new { error = "Nenhum cliente encontrado." });

            var clients = await _clientRepository.GetAllClientsAsync(page * pageSize, pageSize);

            var result = new ListClientsViewModel
            {
                Total = count,
                Page = page,
                PageSize = pageSize,
                Clients = clients
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpGet("/client/{id}")]
    [ProducesResponseType(typeof(ClientViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> GetClientAsync([FromRoute] int id)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
                return NotFound(new { error = "Nenhum cliente encontrado." });

            return Ok(client);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpPut("/client/{id}")]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]
    public async Task<IActionResult> UpdateClientAsync([FromRoute] int id, [FromBody] Client model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
                return NotFound(new { error = "Nenhum cliente encontrado." });

            if (model.Email != client.Email)
            {
                var existingEmailClient = await _clientRepository.GetClientByEmailAsync(model.Email ?? "");
                if (existingEmailClient != null)
                    return BadRequest(new { error = "E-mail já cadastrado." });
            }

            if (model.CPF != client.CPF)
            {
                var existingCPFClient = await _clientRepository.GetClientByCPFAsync(model.CPF ?? "");
                if (existingCPFClient != null)
                    return BadRequest(new { error = "CPF já cadastrado." });
            }

            await _clientRepository.UpdateClientByIdAsync(model, id);

            return Ok(new { Message = "Dados editados com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor." });
        }
    }

    [HttpDelete("/client/{id}")]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> DeleteClientAsync([FromRoute] int id)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var client = await _clientRepository.GetClientByIdAsync(id);

            if (client == null)
                return NotFound(new { error = "Nenhum cliente encontrado." });

            if (client.Status != "Em dia")
                return BadRequest(new { error = "Este cliente não pode ser excluído." });

            await _clientRepository.DeleteClientByIdAsync(id);

            return Ok(new { Message = "Cliente excluído com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }
}

