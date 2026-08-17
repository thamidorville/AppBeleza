namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class ServicoController : ControllerBase
{
    private readonly IServicoService _service;

    public ServicoController(IServicoService service)
    {
        _service = service;
    }

    [HttpGet("meus-servicos")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterMeusServicos()
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(emailUsuarioToken) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        try
        {
            var servicos = await _service.ObterMeusServicos(emailUsuarioToken);

            var response = servicos.Select(s => new ServicoResponseDTO
            {
                Id = HashIdHelper.Encode(s.Id),
                UsuarioId = HashIdHelper.Encode(s.UsuarioId),
                Nome = s.Nome,
                Descricao = s.Descricao,
                Preco = s.Preco,
                DuracaoMinutos = s.DuracaoMinutos,
                Ativo = s.Ativo,
                CriadoEm = s.CriadoEm
            });

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("usuario/{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterTodosPorUsuarioId(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(emailUsuarioToken) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        try
        {
            var servicos = await _service.ObterTodosPorUsuarioId(usuarioId, emailUsuarioToken, role);

            var response = servicos.Select(s => new ServicoResponseDTO
            {
                Id = HashIdHelper.Encode(s.Id),
                UsuarioId = HashIdHelper.Encode(s.UsuarioId),
                Nome = s.Nome,
                Descricao = s.Descricao,
                Preco = s.Preco,
                DuracaoMinutos = s.DuracaoMinutos,
                Ativo = s.Ativo,
                CriadoEm = s.CriadoEm
            });

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var servicoId = HashIdHelper.Decode(id);
        if (servicoId == 0) return BadRequest("Id inválido.");

        var servico = await _service.ObterPorId(servicoId);
        if (servico == null) return NotFound("Serviço não encontrado.");

        var response = new ServicoResponseDTO
        {
            Id = HashIdHelper.Encode(servico.Id),
            UsuarioId = HashIdHelper.Encode(servico.UsuarioId),
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Preco = servico.Preco,
            DuracaoMinutos = servico.DuracaoMinutos,
            Ativo = servico.Ativo,
            CriadoEm = servico.CriadoEm
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Criar([FromBody] ServicoRequestDTO request)
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        var servico = new Servico
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            DuracaoMinutos = request.DuracaoMinutos
        };

        var id = await _service.Criar(servico, emailUsuarioToken);

        var response = new ServicoResponseDTO
        {
            Id = HashIdHelper.Encode(id),
            UsuarioId = "",
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Preco = servico.Preco,
            DuracaoMinutos = servico.DuracaoMinutos,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        return Created($"api/servico/{response.Id}", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] ServicoRequestDTO request)
    {
        var servicoId = HashIdHelper.Decode(id);
        if (servicoId == 0) return BadRequest("Id inválido.");

        var servico = new Servico
        {
            Id = servicoId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            DuracaoMinutos = request.DuracaoMinutos
        };

        await _service.Atualizar(servico);
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Desativar(string id)
    {
        var servicoId = HashIdHelper.Decode(id);
        if (servicoId == 0) return BadRequest("Id inválido.");

        await _service.Desativar(servicoId);
        return Ok(new { message = "Serviço desativado com sucesso." });
    }
}
