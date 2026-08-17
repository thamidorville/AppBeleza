namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObterTodos();
    Task<Cliente?> ObterPorId(int id);
    Task<int> Criar(Cliente cliente);
    Task Atualizar(Cliente cliente);
    Task Deletar(int id);
}
