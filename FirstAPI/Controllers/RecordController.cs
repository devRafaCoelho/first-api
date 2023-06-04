using FirstAPI.Extensions;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Repositories.Contracts;
using FirstAPI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers;

[ApiController]
[Authorize]

public class RecordController : ControllerBase
{
    private readonly IRecordRepository _recordRepository;

    public RecordController(IRecordRepository recordRepository)
        => _recordRepository = recordRepository;

    [HttpPost("/record/register")]
    [ProducesResponseType(typeof(Client), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> RegisterRecordAsync([FromBody] Record model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var record = await _recordRepository.AddRecordAsync(model);

            var result = new Record
            {
                Id = record.Id,
                Id_Client = model.Id_Client,
                Description = model.Description,
                Due_Date = model.Due_Date,
                Value = model.Value,
                Paid_Out = model.Paid_Out
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpGet("/record")]
    [ProducesResponseType(typeof(List<RecordViewModel>), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> ListRecordsAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 25)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var count = await _recordRepository.GetCountRecordsAsync();

            if (count == 0)
                return NotFound(new { error = "Nenhuma cobrança encontrada." });

            var records = await _recordRepository.GetAllRecordsAsync(page * pageSize, pageSize);

            var result = new ListRecordsViewModel

            {
                Total = count,
                Page = page,
                PageSize = pageSize,
                Records = records
            };

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpGet("/record/{id}")]
    [ProducesResponseType(typeof(RecordViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> GetRecordAsync([FromRoute] int id)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var record = await _recordRepository.GetRecordByIdAsync(id);

            if (record == null)
                return NotFound(new { error = "Nenhuma cobrança encontrada." });

            return Ok(record);
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }

    [HttpPut("/record/{id}")]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]
    public async Task<IActionResult> UpdateRecordAsync([FromRoute] int id, [FromBody] Record model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var record = await _recordRepository.GetRecordByIdAsync(id);

            if (record == null)
                return NotFound(new { error = "Nenuhuma cobrança encontrada." });

            await _recordRepository.UpdateRecordByIdAsync(model, id);

            return Ok(new { Message = "Dados editados com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor." });
        }
    }

    [HttpDelete("/record/{id}")]
    [ProducesResponseType(typeof(MessageViewModel), 200)]
    [ProducesResponseType(typeof(ErrorViewModel), 400)]
    [ProducesResponseType(typeof(ErrorViewModel), 401)]
    [ProducesResponseType(typeof(ErrorViewModel), 404)]
    [ProducesResponseType(typeof(ErrorViewModel), 500)]

    public async Task<IActionResult> DeleteRecordAsync([FromRoute] int id)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = ModelState.GetErrors() });

            var record = await _recordRepository.GetRecordByIdAsync(id);

            if (record == null)
                return NotFound(new { error = "Nenuhuma cobrança encontrada." });

            if (record.Status != "Pendente")
                return BadRequest(new { error = "Esta cobrança não pode ser excluída." });

            await _recordRepository.DeleteRecordByIdAsync(id);

            return Ok(new { Message = "Cobrança excluída com sucesso!" });
        }
        catch
        {
            return StatusCode(500, new { error = "Falha interna no servidor" });
        }
    }
}

