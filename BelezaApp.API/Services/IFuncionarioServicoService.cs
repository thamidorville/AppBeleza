namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IFuncionarioServicoService
{
    Task<IEnumerable<Servico>> ObterServicosPorFuncionarioId(int funcionarioId);
    Task<IEnumerable<Funcionario>> ObterFuncionariosPorServicoId(int servicoId);
    Task<int> Vincular(FuncionarioServico funcionarioServico);
    Task Desvincular(int funcionarioId, int servicoId);
}
