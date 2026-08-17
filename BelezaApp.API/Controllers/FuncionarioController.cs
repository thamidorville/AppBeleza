namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class FuncionarioController : ControllerBase
{
    private readonly IFuncionarioService _service;

    public FuncionarioController(IFuncionarioService service)
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

        var funcionarios = await _service.ObterTodosPorUsuarioId(usuarioId);

        var response = funcionarios.Select(f => new FuncionarioResponseDTO
        {
            Id = HashIdHelper.Encode(f.Id),
            UsuarioId = HashIdHelper.Encode(f.UsuarioId),
            Nome = f.Nome,
            Email = f.Email,
            Telefone = f.Telefone,
            Ativo = f.Ativo,
            CriadoEm = f.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Gestor,Funcionario")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id inválido.");

        var funcionario = await _service.ObterPorId(funcionarioId);
        if (funcionario == null) return NotFound("Funcionário não encontrado.");

        var response = new FuncionarioResponseDTO
        {
            Id = HashIdHelper.Encode(funcionario.Id),
            UsuarioId = HashIdHelper.Encode(funcionario.UsuarioId),
            Nome = funcionario.Nome,
            Email = funcionario.Email,
            Telefone = funcionario.Telefone,
            Ativo = funcionario.Ativo,
            CriadoEm = funcionario.CriadoEm
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Gestor")]
    public async Task<IActionResult> Criar([FromBody] FuncionarioRequestDTO request)
    {
        var usuarioId = HashIdHelper.Decode(request.UsuarioId);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var funcionario = new Funcionario
        {
            UsuarioId = usuarioId,
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha,
            Telefone = request.Telefone
        };

        var id = await _service.Criar(funcionario);

        var response = new FuncionarioResponseDTO
        {
            Id = HashIdHelper.Encode(id),
            UsuarioId = request.UsuarioId,
            Nome = funcionario.Nome,
            Email = funcionario.Email,
            Telefone = funcionario.Telefone,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        return Created($"api/funcionario/{response.Id}", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Gestor,Funcionario")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] FuncionarioRequestDTO request)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id inválido.");

        var funcionario = new Funcionario
        {
            Id = funcionarioId,
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone
        };

        await _service.Atualizar(funcionario);
        return NoContent();
    }

    [HttpPatch("{id}/desativar")]
    [Authorize(Roles = "Gestor")]
    public async Task<IActionResult> Desativar(string id)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id inválido.");

        await _service.Desativar(funcionarioId);
        return Ok(new { message = "Funcionário desativado com sucesso." });
    }
}
