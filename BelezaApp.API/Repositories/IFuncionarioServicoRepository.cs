namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IFuncionarioServicoRepository
{
    Task<IEnumerable<Servico>> ObterServicosPorFuncionarioId(int funcionarioId);
    Task<IEnumerable<Funcionario>> ObterFuncionariosPorServicoId(int servicoId);
    Task<bool> VinculoJaExiste(int funcionarioId, int servicoId);
    Task<int> Vincular(FuncionarioServico funcionarioServico);
    Task Desvincular(int funcionarioId, int servicoId);
}
