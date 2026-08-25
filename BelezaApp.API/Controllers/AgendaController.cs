namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AgendaController : ControllerBase
{
    private readonly IAgendaService _service;

    public AgendaController(IAgendaService service)
    {
        _service = service;
    }

    [HttpGet("funcionario/{id}")]
    public async Task<IActionResult> ObterPorFuncionarioId(string id)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");

        var agendas = await _service.ObterPorFuncionarioId(funcionarioId);

        var response = agendas.Select(a => new AgendaResponseDTO
        {
            Id = HashIdHelper.Encode(a.Id),
            FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
            UsuarioId = HashIdHelper.Encode(a.UsuarioId),
            ServicoId = HashIdHelper.Encode(a.ServicoId),
            DataAgendamento = a.DataAgendamento,
            HoraInicio = a.HoraInicio,
            HoraFim = a.HoraFim,
            Disponivel = a.Disponivel,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

//esse endpoint vai servir pro cliente listar a agenda de um profissional em específico
    [HttpGet("usuario/{id}")]
    public async Task<IActionResult> ObterPorUsuarioId(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var agendas = await _service.ObterPorUsuarioId(usuarioId);

        var response = agendas.Select(a => new AgendaResponseDTO
        {
            Id = HashIdHelper.Encode(a.Id),
            FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
            FuncionarioNome = a.FuncionarioNome,
            UsuarioId = HashIdHelper.Encode(a.UsuarioId),
            ServicoId = HashIdHelper.Encode(a.ServicoId),
            ServicoNome = a.ServicoNome,
            DataAgendamento = a.DataAgendamento,
            HoraInicio = a.HoraInicio,
            HoraFim = a.HoraFim,
            Disponivel = a.Disponivel,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("vitrine-do-cliente")]
    [Authorize(Roles = "Cliente,Admin")]
    public async Task<IActionResult> ObterVitrineDoCliente()
    {
        var agendas = await _service.ObterVitrineDoCliente();

        var response = agendas.Select(a => new AgendaResponseDTO
        {
            Id = HashIdHelper.Encode(a.Id),
            FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
            FuncionarioNome = a.FuncionarioNome,
            UsuarioId = HashIdHelper.Encode(a.UsuarioId),
            UsuarioNome = a.UsuarioNome,
            ServicoId = HashIdHelper.Encode(a.ServicoId),
            ServicoNome = a.ServicoNome,
            DataAgendamento = a.DataAgendamento,
            HoraInicio = a.HoraInicio,
            HoraFim = a.HoraFim,
            Disponivel = a.Disponivel,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("minha-agenda")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterMinhaAgenda()
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        try
        {
            var agendas = await _service.ObterMinhaAgenda(emailUsuarioToken);

            var response = agendas.Select(a => new AgendaResponseDTO
            {
                Id = HashIdHelper.Encode(a.Id),
                FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
                FuncionarioNome = a.FuncionarioNome,
                UsuarioId = HashIdHelper.Encode(a.UsuarioId),
                ServicoId = HashIdHelper.Encode(a.ServicoId),
                ServicoNome = a.ServicoNome,
                DataAgendamento = a.DataAgendamento,
                HoraInicio = a.HoraInicio,
                HoraFim = a.HoraFim,
                Disponivel = a.Disponivel,
                CriadoEm = a.CriadoEm
            });

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Criar([FromBody] AgendaRequestDTO request)
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        int? funcionarioId = null;

        if (!string.IsNullOrEmpty(request.FuncionarioId))
        {
            funcionarioId = HashIdHelper.Decode(request.FuncionarioId);
            if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");
        }

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        var agenda = new Agenda
        {
            FuncionarioId = funcionarioId,
            ServicoId = servicoId,
            DataAgendamento = request.DataAgendamento,
            HoraInicio = request.HoraInicio,
            HoraFim = request.HoraFim
        };

        try
        {
            var id = await _service.Criar(agenda, emailUsuarioToken);

            var response = new AgendaResponseDTO
            {
                Id = HashIdHelper.Encode(id),
                FuncionarioId = request.FuncionarioId,
                UsuarioId = "",
                ServicoId = request.ServicoId,
                DataAgendamento = request.DataAgendamento,
                HoraInicio = request.HoraInicio,
                HoraFim = request.HoraFim,
                Disponivel = true,
                CriadoEm = DateTime.UtcNow
            };

            return Created($"api/agenda/{response.Id}", response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] AgendaRequestDTO request)
    {
        var agendaId = HashIdHelper.Decode(id);
        if (agendaId == 0) return BadRequest("Id inválido.");

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        var agenda = new Agenda
        {
            Id = agendaId,
            ServicoId = servicoId,
            DataAgendamento = request.DataAgendamento,
            HoraInicio = request.HoraInicio,
            HoraFim = request.HoraFim,
            Disponivel = true
        };

        await _service.Atualizar(agenda);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var agendaId = HashIdHelper.Decode(id);
        if (agendaId == 0) return BadRequest("Id inválido.");

        await _service.Deletar(agendaId);
        return Ok(new { message = "Deletado com sucesso." });
    }
}
