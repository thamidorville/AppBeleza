namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _service;

    public UsuarioController(IUsuarioService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterTodos()
    {
        var usuarios = await _service.ObterTodos();

        var response = usuarios.Select(u => new UsuarioResponseDTO
        {
            Id = HashIdHelper.Encode(u.Id), //transforma o int em string codificada
            Nome = u.Nome,
            Email = u.Email,
            TipoPessoa = u.TipoPessoa,
            Documento = u.Documento,
            Telefone = u.Telefone,
            Role = u.Role,
            CriadoEm = u.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id inválido.");

        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(emailUsuarioToken) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        if (role != "Admin")
        {
            var usuarioToken = await _service.ObterPorEmail(emailUsuarioToken);
            if (usuarioToken == null || usuarioToken.Id != usuarioId)
                return Forbid();
        }

        var usuario = await _service.ObterPorId(usuarioId);
        if (usuario == null) return NotFound("Usuário não encontrado.");

        var response = new UsuarioResponseDTO
        {
            Id = HashIdHelper.Encode(usuario.Id),
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoPessoa = usuario.TipoPessoa,
            Documento = usuario.Documento,
            Telefone = usuario.Telefone,
            Role = usuario.Role,
            CriadoEm = usuario.CriadoEm
        };

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Criar([FromBody] UsuarioRequestDTO request)
    {
        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha,
            TipoPessoa = request.TipoPessoa,
            Documento = request.Documento,
            Telefone = request.Telefone,
            Role = request.Role
        };

        var id = await _service.Criar(usuario);

        var response = new UsuarioResponseDTO
        {
            Id = HashIdHelper.Encode(id),
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoPessoa = usuario.TipoPessoa,
            Documento = usuario.Documento,
            Telefone = usuario.Telefone,
            Role = usuario.Role,
            CriadoEm = DateTime.UtcNow
        };

        return Created($"api/usuario/{response.Id}", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] UsuarioRequestDTO request)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id inválido.");

        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(emailUsuarioToken) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        if (role != "Admin")
        {
            var usuarioToken = await _service.ObterPorEmail(emailUsuarioToken);
            if (usuarioToken == null || usuarioToken.Id != usuarioId)
                return Forbid();
        }

        var usuario = new Usuario
        {
            Id = usuarioId,
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone
        };

        await _service.Atualizar(usuario);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id inválido.");

        await _service.Deletar(usuarioId);
        return Ok(new { message = "Deletado com sucesso." });
    }
}
