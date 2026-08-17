namespace BelezaApp.API.Services;

using BelezaApp.API.Helpers;
using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public FuncionarioService(IFuncionarioRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId, string emailUsuarioToken, string role)
    {
        if (role != "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
            if (usuario == null || usuario.Id != usuarioId)
                throw new UnauthorizedAccessException("Acesso negado.");
        }

        return await _repository.ObterTodosPorUsuarioId(usuarioId);
    }

    public async Task<Funcionario?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Funcionario funcionario, string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        funcionario.UsuarioId = usuario.Id;
        funcionario.Senha = SenhaHelper.GerarHash(funcionario.Senha);
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

    public async Task<Usuario?> ObterGestorPorEmail(string email)
    {
        return await _usuarioRepository.ObterPorEmail(email);
    }
}
