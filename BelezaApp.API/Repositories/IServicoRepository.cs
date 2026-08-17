namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IServicoRepository
{
    Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId);
    Task<Servico?> ObterPorId(int id);
    Task<int> Criar(Servico servico);
    Task Atualizar(Servico servico);
    Task Desativar(int id);
}
