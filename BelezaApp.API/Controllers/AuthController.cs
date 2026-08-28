namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.DTOs;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEnderecoService _enderecoService;

    public AuthController(IAuthService authService, IEnderecoService enderecoService)
    {
        _authService = authService;
        _enderecoService = enderecoService;
    }

    [HttpPost("cadastro/usuario")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CadastrarUsuario([FromBody] UsuarioRequestDTO request)
    {
        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha,
            TipoPessoa = request.TipoPessoa,
            Documento = request.Documento,
            Telefone = request.Telefone
        };

        try
        {
            var id = await _authService.CadastrarUsuario(usuario);
            return Created("", new { id = HashIdHelper.Encode(id), message = "Cadastro realizado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("cadastro/cliente")]
    public async Task<IActionResult> CadastrarCliente([FromBody] ClienteRequestDTO request)
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
            var clienteId = await _authService.CadastrarCliente(cliente);

            if (!string.IsNullOrEmpty(request.Cep))
            {
                var endereco = new Endereco
                {
                    ClienteId = clienteId,
                    Logradouro = request.Logradouro ?? "",
                    Numero = request.Numero ?? "",
                    Complemento = request.Complemento,
                    Bairro = request.Bairro ?? "",
                    Cidade = request.Cidade ?? "",
                    Estado = request.Estado ?? "",
                    Cep = request.Cep
                };
                await _enderecoService.CriarDireto(endereco);
            }

            return Created("", new { message = "Cadastro realizado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var token = await _authService.Login(request.Email, request.Senha);
            return Ok(new { token });
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPut("alterar-senha")]
    [Authorize]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequestDTO request)
    {
        var email = TokenClaimsHelper.ObterEmail(User);
        var role = TokenClaimsHelper.ObterRole(User);

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
            return Unauthorized("Token inválido.");

        try
        {
            await _authService.AlterarSenha(email, role, request.SenhaAtual, request.NovaSenha);
            return Ok(new { message = "Senha alterada com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
