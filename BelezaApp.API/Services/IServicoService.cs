namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IServicoService
{
    Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId, string emailUsuarioToken, string role);
    Task<IEnumerable<Servico>> ObterMeusServicos(string emailUsuarioToken);
    Task<Servico?> ObterPorId(int id);
    Task<int> Criar(Servico servico, string emailUsuarioToken);
    Task Atualizar(Servico servico);
    Task Desativar(int id);
}
