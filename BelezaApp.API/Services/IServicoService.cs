namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IServicoService
{
    Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId);
    Task<Servico?> ObterPorId(int id);
    Task<int> Criar(Servico servico);
    Task Atualizar(Servico servico);
    Task Desativar(int id);
}
