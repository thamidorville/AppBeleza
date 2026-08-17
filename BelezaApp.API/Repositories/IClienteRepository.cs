namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ObterTodos();
    Task<Cliente?> ObterPorId(int id);
    Task<Cliente?> ObterPorEmail(string email);
    Task<int> Criar(Cliente cliente);
    Task Atualizar(Cliente cliente);
    Task AtualizarSenha(int id, string senha);
    Task Deletar(int id);
}
