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

    [HttpGet("usuario/{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterTodosPorUsuarioId(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var tokenUsuarioId = TokenClaimsHelper.ObterUsuarioId(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (tokenUsuarioId != usuarioId && role != "Admin")
            return Forbid();

        var servicos = await _service.ObterTodosPorUsuarioId(usuarioId);

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
        var usuarioId = HashIdHelper.Decode(request.UsuarioId);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var servico = new Servico
        {
            UsuarioId = usuarioId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Preco = request.Preco,
            DuracaoMinutos = request.DuracaoMinutos
        };

        var id = await _service.Criar(servico);

        var response = new ServicoResponseDTO
        {
            Id = HashIdHelper.Encode(id),
            UsuarioId = request.UsuarioId,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Preco = servico.Preco,
            DuracaoMinutos = servico.DuracaoMinutos,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };
        
        Console.WriteLine($"[SERVICO] Criado. Id real: {id}, HashId: {response.Id}");
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
