namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IFuncionarioService
{
    Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId, string emailUsuarioToken, string role);
    Task<Funcionario?> ObterPorId(int id);
    Task<int> Criar(Funcionario funcionario, string emailUsuarioToken);
    Task Atualizar(Funcionario funcionario);
    Task Desativar(int id);
    Task<Usuario?> ObterGestorPorEmail(string email);
}
