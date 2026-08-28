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
    private readonly IViaCepService _viaCepService;

    public EnderecoController(IEnderecoService service, IViaCepService viaCepService)
    {
        _service = service;
        _viaCepService = viaCepService;
    }

    [HttpGet("consulta-cep/{cep}")]
    public async Task<IActionResult> ConsultarCep(string cep)
    {
        var resultado = await _viaCepService.ConsultarCep(cep);
        if (resultado == null)
            return BadRequest("CEP não encontrado.");

        return Ok(new
        {
            logradouro = resultado.Logradouro,
            bairro = resultado.Bairro,
            cidade = resultado.Localidade,
            estado = resultado.Uf
        });
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
                UsuarioId = endereco.UsuarioId.HasValue ? HashIdHelper.Encode(endereco.UsuarioId.Value) : "",
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

    [HttpPost("usuario/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CriarParaUsuario(string id, [FromBody] EnderecoRequestDTO request)
    {
        var usuarioId = HashIdHelper.Decode(id);
        if (usuarioId == 0) return BadRequest("Id de usuário inválido.");

        var endereco = new Endereco
        {
            UsuarioId = usuarioId,
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
            var enderecoJaExiste = await _service.ObterPorUsuarioId(usuarioId, "", "Admin");
            if (enderecoJaExiste != null)
                return BadRequest("Usuário já possui endereço cadastrado.");

            var novoId = await _service.CriarDireto(endereco);
            return Created($"api/endereco/{HashIdHelper.Encode(novoId)}", new { id = HashIdHelper.Encode(novoId) });
        }
        catch (Exception ex)
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

    [HttpGet("meu-endereco")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> ObterMeuEndereco()
    {
        var emailClienteToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailClienteToken))
            return Unauthorized("Token inválido.");

        try
        {
            var endereco = await _service.ObterPorClienteId(emailClienteToken);
            if (endereco == null) return NotFound("Endereço não encontrado.");

            var response = new EnderecoResponseDTO
            {
                Id = HashIdHelper.Encode(endereco.Id),
                UsuarioId = "",
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

    [HttpPost("cliente")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> CriarParaCliente([FromBody] EnderecoRequestDTO request)
    {
        var emailClienteToken = TokenClaimsHelper.ObterEmail(User);
        if (string.IsNullOrEmpty(emailClienteToken))
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
            var id = await _service.CriarParaCliente(endereco, emailClienteToken);
            return Created($"api/endereco/{HashIdHelper.Encode(id)}", new { id = HashIdHelper.Encode(id) });
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
