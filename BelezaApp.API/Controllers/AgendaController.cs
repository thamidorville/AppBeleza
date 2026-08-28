namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;
using BelezaApp.API.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AgendaController : ControllerBase
{
    private readonly IAgendaService _service;
    private readonly IEnderecoRepository _enderecoRepository;

    public AgendaController(IAgendaService service, IEnderecoRepository enderecoRepository)
    {
        _service = service;
        _enderecoRepository = enderecoRepository;
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
            Status = a.Status,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

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
            Status = a.Status,
            CriadoEm = a.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("vitrine-do-cliente")]
    [Authorize(Roles = "Cliente,Admin")]
    public async Task<IActionResult> ObterVitrineDoCliente([FromQuery] decimal? lat, [FromQuery] decimal? lng)
    {
        var agendas = await _service.ObterVitrineDoCliente();

        var usuarioIds = agendas.Select(a => a.UsuarioId).Distinct().ToList();
        var enderecos = new Dictionary<int, Endereco>();

        foreach (var usuarioId in usuarioIds)
        {
            var endereco = await _enderecoRepository.ObterPorUsuarioId(usuarioId);
            if (endereco != null)
                enderecos[usuarioId] = endereco;
        }

        var response = agendas.Select(a =>
        {
            enderecos.TryGetValue(a.UsuarioId, out var endereco);

            double? distancia = null;
            if (lat.HasValue && lng.HasValue && endereco?.Latitude != null && endereco?.Longitude != null)
            {
                distancia = DistanciaHelper.CalcularDistanciaKm(lat.Value, lng.Value, endereco.Latitude.Value, endereco.Longitude.Value);
                distancia = Math.Round(distancia.Value, 1);
            }

            return new AgendaResponseDTO
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
                Status = a.Status,
                EnderecoBairro = endereco?.Bairro,
                EnderecoCidade = endereco?.Cidade,
                DistanciaKm = distancia,
                CriadoEm = a.CriadoEm
            };
        })
        .OrderBy(a => a.DistanciaKm ?? double.MaxValue)
        .ToList();

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
                Status = "Disponivel",
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
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        var agendaId = HashIdHelper.Decode(id);
        if (agendaId == 0) return BadRequest("Id inválido.");

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        int? funcionarioId = null;
        if (!string.IsNullOrEmpty(request.FuncionarioId))
        {
            funcionarioId = HashIdHelper.Decode(request.FuncionarioId);
            if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");
        }

        var agenda = new Agenda
        {
            Id = agendaId,
            FuncionarioId = funcionarioId,
            ServicoId = servicoId,
            DataAgendamento = request.DataAgendamento,
            HoraInicio = request.HoraInicio,
            HoraFim = request.HoraFim
        };

        try
        {
            await _service.Atualizar(agenda, emailUsuarioToken);
            return NoContent();
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        var agendaId = HashIdHelper.Decode(id);
        if (agendaId == 0) return BadRequest("Id inválido.");

        try
        {
            await _service.Deletar(agendaId, emailUsuarioToken);
            return Ok(new { message = "Deletado com sucesso." });
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
}
