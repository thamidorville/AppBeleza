namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _repository;

    public FuncionarioService(IFuncionarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId)
    {
        return await _repository.ObterTodosPorUsuarioId(usuarioId);
    }

    public async Task<Funcionario?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Funcionario funcionario)
    {
        funcionario.Senha = Helpers.SenhaHelper.GerarHash(funcionario.Senha);
        return await _repository.Criar(funcionario);
    }

    public async Task Atualizar(Funcionario funcionario)
    {
        await _repository.Atualizar(funcionario);
    }

    public async Task Desativar(int id)
    {
        await _repository.Desativar(id);
    }
}
