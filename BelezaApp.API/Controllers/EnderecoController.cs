namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class EnderecoController : ControllerBase
{
    private readonly IEnderecoService _service;

    public EnderecoController(IEnderecoService service)
    {
        _service = service;
    }

    [HttpGet("usuario/{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> ObterPorUsuarioId(string id)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(emailUsuarioToken) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        try
        {
            var endereco = await _service.ObterPorUsuarioId(usuarioId, emailUsuarioToken, role);
            if (endereco == null) return NotFound("Endereço não encontrado.");

            var response = new EnderecoResponseDTO
            {
                Id = HashIdHelper.Encode(endereco.Id),
                UsuarioId = HashIdHelper.Encode(endereco.UsuarioId),
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep,
                Latitude = endereco.Latitude,
                Longitude = endereco.Longitude,
                CriadoEm = endereco.CriadoEm
            };

            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Criar([FromBody] EnderecoRequestDTO request)
    {
        var emailUsuarioToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailUsuarioToken))
            return Unauthorized("Token inválido.");

        var endereco = new Endereco
        {
            Logradouro = request.Logradouro,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Cep = request.Cep
        };

        try
        {
            var id = await _service.Criar(endereco, emailUsuarioToken);

            var response = new EnderecoResponseDTO
            {
                Id = HashIdHelper.Encode(id),
                UsuarioId = "",
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep,
                Latitude = null,
                Longitude = null,
                CriadoEm = DateTime.UtcNow
            };

            return Created($"api/endereco/{response.Id}", response);
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
    public async Task<IActionResult> Atualizar(string id, [FromBody] EnderecoRequestDTO request)
    {
        var enderecoId = HashIdHelper.Decode(id);
        if (enderecoId == 0) return BadRequest("Id inválido.");

        var endereco = new Endereco
        {
            Id = enderecoId,
            Logradouro = request.Logradouro,
            Numero = request.Numero,
            Complemento = request.Complemento,
            Bairro = request.Bairro,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Cep = request.Cep
        };

        await _service.Atualizar(endereco);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Deletar(string id)
    {
        var enderecoId = HashIdHelper.Decode(id);
        if (enderecoId == 0) return BadRequest("Id inválido.");

        await _service.Deletar(enderecoId);
        return Ok(new { message = "Deletado com sucesso." });
    }
}
