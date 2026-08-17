namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> ObterTodos();
    Task<Usuario?> ObterPorId(int id);
    Task<Usuario?> ObterPorEmail(string email);
    Task<int> Criar(Usuario usuario);
    Task Atualizar(Usuario usuario);
    Task AtualizarSenha(int id, string senha);
    Task Deletar(int id);
}
