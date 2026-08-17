namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IFuncionarioRepository
{
    Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId);
    Task<Funcionario?> ObterPorId(int id);
    Task<Funcionario?> ObterPorEmail(string email);
    Task<int> Criar(Funcionario funcionario);
    Task Atualizar(Funcionario funcionario);
    Task AtualizarSenha(int id, string senha);
    Task Desativar(int id);
}
