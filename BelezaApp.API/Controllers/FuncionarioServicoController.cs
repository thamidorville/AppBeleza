namespace BelezaApp.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Services;
using BelezaApp.API.Helpers;

[ApiController]
[Route("api/[controller]")]
public class FuncionarioServicoController : ControllerBase
{
    private readonly IFuncionarioServicoService _service;

    public FuncionarioServicoController(IFuncionarioServicoService service)
    {
        _service = service;
    }

    [HttpGet("funcionario/{id}")]
    public async Task<IActionResult> ObterServicosPorFuncionarioId(string id)
    {
        var funcionarioId = HashIdHelper.Decode(id);
        if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");

        var servicos = await _service.ObterServicosPorFuncionarioId(funcionarioId);

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

    [HttpGet("servico/{id}")]
    public async Task<IActionResult> ObterFuncionariosPorServicoId(string id)
    {
        var servicoId = HashIdHelper.Decode(id);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        var funcionarios = await _service.ObterFuncionariosPorServicoId(servicoId);

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

    [HttpPost]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Vincular([FromBody] FuncionarioServicoRequestDTO request)
    {
        var funcionarioId = HashIdHelper.Decode(request.FuncionarioId);
        if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        var funcionarioServico = new FuncionarioServico
        {
            FuncionarioId = funcionarioId,
            ServicoId = servicoId
        };

        try
        {
            await _service.Vincular(funcionarioServico);
            return Created("", new { message = "Funcionário vinculado ao serviço com sucesso." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Gestor,Admin")]
    public async Task<IActionResult> Desvincular([FromBody] FuncionarioServicoRequestDTO request)
    {
        var funcionarioId = HashIdHelper.Decode(request.FuncionarioId);
        if (funcionarioId == 0) return BadRequest("Id de funcionário inválido.");

        var servicoId = HashIdHelper.Decode(request.ServicoId);
        if (servicoId == 0) return BadRequest("Id de serviço inválido.");

        await _service.Desvincular(funcionarioId, servicoId);
        return Ok(new { message = "Funcionário desvinculado do serviço com sucesso." });
    }
}
