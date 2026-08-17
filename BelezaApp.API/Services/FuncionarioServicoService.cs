namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class FuncionarioServicoService : IFuncionarioServicoService
{
    private readonly IFuncionarioServicoRepository _repository;

    public FuncionarioServicoService(IFuncionarioServicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Servico>> ObterServicosPorFuncionarioId(int funcionarioId)
    {
        return await _repository.ObterServicosPorFuncionarioId(funcionarioId);
    }

    public async Task<IEnumerable<Funcionario>> ObterFuncionariosPorServicoId(int servicoId)
    {
        return await _repository.ObterFuncionariosPorServicoId(servicoId);
    }

    public async Task<int> Vincular(FuncionarioServico funcionarioServico)
    {
        var vinculoJaExiste = await _repository.VinculoJaExiste(funcionarioServico.FuncionarioId, funcionarioServico.ServicoId);
        if (vinculoJaExiste)
            throw new InvalidOperationException("Este funcionário já está vinculado a este serviço.");

        return await _repository.Vincular(funcionarioServico);
    }

    public async Task Desvincular(int funcionarioId, int servicoId)
    {
        await _repository.Desvincular(funcionarioId, servicoId);
    }
}
