namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IFuncionarioService
{
    Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId);
    Task<Funcionario?> ObterPorId(int id);
    Task<int> Criar(Funcionario funcionario);
    Task Atualizar(Funcionario funcionario);
    Task Desativar(int id);
}
