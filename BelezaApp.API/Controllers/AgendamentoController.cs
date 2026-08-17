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
        var agendaId = HashIdHelper.Decode(request.AgendaId);
        if (agendaId == 0) return BadRequest("Id de agenda inválido.");

        var clienteId = HashIdHelper.Decode(request.ClienteId);
        if (clienteId == 0) return BadRequest("Id de cliente inválido.");

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        var usuarioId = HashIdHelper.Decode(request.UsuarioId);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        int? funcionarioId = null;
        if (!string.IsNullOrEmpty(request.FuncionarioId))
        {
            funcionarioId = HashIdHelper.Decode(request.FuncionarioId);
            if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");
        }

        var agendamento = new Agendamento
        {
            AgendaId = agendaId,
            ClienteId = clienteId,
            ServicoId = servicoId,
            FuncionarioId = funcionarioId,
            UsuarioId = usuarioId,
            Data = request.Data,
            HorarioInicio = request.HorarioInicio,
            HorarioFim = request.HorarioFim
        };

        try
        {
            var id = await _service.Criar(agendamento);

            var response = new AgendamentoResponseDTO
            {
                Id = HashIdHelper.Encode(id),
                AgendaId = request.AgendaId,
                ClienteId = request.ClienteId,
                ServicoId = request.ServicoId,
                FuncionarioId = request.FuncionarioId,
                UsuarioId = request.UsuarioId,
                Data = request.Data,
                HorarioInicio = request.HorarioInicio,
                HorarioFim = request.HorarioFim,
                Status = "Confirmado",
                CriadoEm = DateTime.UtcNow
            };

            return Created($"api/agendamento/{response.Id}", response);
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
