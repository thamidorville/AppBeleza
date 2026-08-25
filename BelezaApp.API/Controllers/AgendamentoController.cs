namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AgendamentoController : ControllerBase
{
    private readonly IAgendamentoService _service;

    public AgendamentoController(IAgendamentoService service)
    {
        _service = service;
    }

    [HttpGet("cliente/{id}")]
    public async Task<IActionResult> ObterPorClienteId(string id)
    {
        var clienteId = HashIdHelper.Decode(id);
        if (clienteId == 0) return BadRequest("Id de cliente inválido.");

        var agendamentos = await _service.ObterPorClienteId(clienteId);

        var response = agendamentos.Select(a => new AgendamentoResponseDTO
        {
            Id = HashIdHelper.Encode(a.Id),
            AgendaId = HashIdHelper.Encode(a.AgendaId),
            ClienteId = HashIdHelper.Encode(a.ClienteId),
            ServicoId = HashIdHelper.Encode(a.ServicoId),
            FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
            UsuarioId = HashIdHelper.Encode(a.UsuarioId),
            Data = a.Data,
            HorarioInicio = a.HorarioInicio,
            HorarioFim = a.HorarioFim,
            Status = a.Status,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("meus-agendamentos")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> ObterMeusAgendamentos()
    {
        var emailClienteToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailClienteToken))
            return Unauthorized("Token inválido.");

        try
        {
            var agendamentos = await _service.ObterMeusAgendamentos(emailClienteToken);

            var response = agendamentos.Select(a => new AgendamentoResponseDTO
            {
                Id = HashIdHelper.Encode(a.Id),
                AgendaId = HashIdHelper.Encode(a.AgendaId),
                ClienteId = HashIdHelper.Encode(a.ClienteId),
                ServicoId = HashIdHelper.Encode(a.ServicoId),
                ServicoNome = a.ServicoNome,
                FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
                FuncionarioNome = a.FuncionarioNome,
                UsuarioId = HashIdHelper.Encode(a.UsuarioId),
                UsuarioNome = a.UsuarioNome,
                Data = a.Data,
                HorarioInicio = a.HorarioInicio,
                HorarioFim = a.HorarioFim,
                Status = a.Status,
                CriadoEm = a.CriadoEm
            });

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("meus-agendamentos-gestor")]
    [Authorize(Roles = "Gestor")]
    public async Task<IActionResult> ObterMeusAgendamentosGestor()
    {
        var emailGestorToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailGestorToken))
            return Unauthorized("Token inválido.");

        try
        {
            var agendamentos = await _service.ObterMeusAgendamentosGestor(emailGestorToken);

            var response = agendamentos.Select(a => new AgendamentoResponseDTO
            {
                Id = HashIdHelper.Encode(a.Id),
                AgendaId = HashIdHelper.Encode(a.AgendaId),
                ClienteId = HashIdHelper.Encode(a.ClienteId),
                ClienteNome = a.ClienteNome,
                ServicoId = HashIdHelper.Encode(a.ServicoId),
                ServicoNome = a.ServicoNome,
                FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
                FuncionarioNome = a.FuncionarioNome,
                UsuarioId = HashIdHelper.Encode(a.UsuarioId),
                UsuarioNome = a.UsuarioNome,
                Data = a.Data,
                HorarioInicio = a.HorarioInicio,
                HorarioFim = a.HorarioFim,
                Status = a.Status,
                CriadoEm = a.CriadoEm
            });

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("funcionario/{id}")]
    public async Task<IActionResult> ObterPorFuncionarioId(string id)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");

        var agendamentos = await _service.ObterPorFuncionarioId(funcionarioId);

        var response = agendamentos.Select(a => new AgendamentoResponseDTO
        {
            Id = HashIdHelper.Encode(a.Id),
            AgendaId = HashIdHelper.Encode(a.AgendaId),
            ClienteId = HashIdHelper.Encode(a.ClienteId),
            ServicoId = HashIdHelper.Encode(a.ServicoId),
            FuncionarioId = a.FuncionarioId.HasValue ? HashIdHelper.Encode(a.FuncionarioId.Value) : null,
            UsuarioId = HashIdHelper.Encode(a.UsuarioId),
            Data = a.Data,
            HorarioInicio = a.HorarioInicio,
            HorarioFim = a.HorarioFim,
            Status = a.Status,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Cliente,Admin")]
    public async Task<IActionResult> Criar([FromBody] AgendamentoRequestDTO request)
    {
        var emailClienteToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailClienteToken))
            return Unauthorized("Token inválido.");

        var agendaId = HashIdHelper.Decode(request.AgendaId);
        if (agendaId == 0) return BadRequest("Id de agenda inválido.");

        try
        {
            var id = await _service.Criar(agendaId, emailClienteToken);
            return Created($"api/agendamento/{HashIdHelper.Encode(id)}", new
            {
                id = HashIdHelper.Encode(id),
                message = "Agendamento realizado com sucesso."
            });
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

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> AtualizarStatus(string id, [FromBody] AgendamentoStatusDTO request)
    {
        var agendamentoId = HashIdHelper.Decode(id);
        if (agendamentoId == 0) return BadRequest("Id inválido.");

        try
        {
            await _service.AtualizarStatus(agendamentoId, request.Status);
            return Ok(new { message = $"Status atualizado para {request.Status}." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
