namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IUsuarioService
{
    Task<IEnumerable<Usuario>> ObterTodos();
    Task<Usuario?> ObterPorId(int id);
    Task<int> Criar(Usuario usuario);
    Task Atualizar(Usuario usuario);
    Task Deletar(int id);
}
