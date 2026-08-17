namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Usuario>> ObterTodos()
    {
        return await _repository.ObterTodos();
    }

    public async Task<Usuario?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<Usuario?> ObterPorEmail(string email)
    {
        return await _repository.ObterPorEmail(email);
    }

    public async Task<int> Criar(Usuario usuario)
    {
        usuario.Senha = Helpers.SenhaHelper.GerarHash(usuario.Senha);
        return await _repository.Criar(usuario);
    }

    public async Task Atualizar(Usuario usuario)
    {
        await _repository.Atualizar(usuario);
    }

    public async Task Deletar(int id)
    {
        await _repository.Deletar(id);
    }
}
