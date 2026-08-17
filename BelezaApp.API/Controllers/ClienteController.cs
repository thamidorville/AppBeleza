namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _service;

    public ClienteController(IClienteService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ObterTodos()
    {
        var clientes = await _service.ObterTodos();

        var response = clientes.Select(c => new ClienteResponseDTO
        {
            Id = HashIdHelper.Encode(c.Id),
            Nome = c.Nome,
            Email = c.Email,
            Telefone = c.Telefone,
            CriadoEm = c.CriadoEm
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var clienteId = HashIdHelper.Decode(id);
        if (clienteId == 0) return BadRequest("Id inválido.");

        var tokenUsuarioId = TokenClaimsHelper.ObterUsuarioId(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (tokenUsuarioId != clienteId && role != "Admin")
            return Forbid();

        var cliente = await _service.ObterPorId(clienteId);
        if (cliente == null) return NotFound("Cliente não encontrado.");

        var response = new ClienteResponseDTO
        {
            Id = HashIdHelper.Encode(cliente.Id),
            Nome = cliente.Nome,
            Email = cliente.Email,
            Telefone = cliente.Telefone,
            CriadoEm = cliente.CriadoEm
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ClienteRequestDTO request)
    {
        var cliente = new Cliente
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha,
            Telefone = request.Telefone
        };

        try
        {
            var id = await _service.Criar(cliente);

            var response = new ClienteResponseDTO
            {
                Id = HashIdHelper.Encode(id),
                Nome = cliente.Nome,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                CriadoEm = DateTime.UtcNow
            };

            return Created($"api/cliente/{response.Id}", response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Cliente,Admin")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] ClienteRequestDTO request)
    {
        var clienteId = HashIdHelper.Decode(id);
        if (clienteId == 0) return BadRequest("Id inválido.");

        var tokenUsuarioId = TokenClaimsHelper.ObterUsuarioId(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (tokenUsuarioId != clienteId && role != "Admin")
            return Forbid();

        var cliente = new Cliente
        {
            Id = clienteId,
            Nome = request.Nome,
            Email = request.Email,
            Telefone = request.Telefone
        };

        await _service.Atualizar(cliente);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var clienteId = HashIdHelper.Decode(id);
        if (clienteId == 0) return BadRequest("Id inválido.");

        await _service.Deletar(clienteId);
        return Ok(new { message = "Deletado com sucesso." });
    }
}
