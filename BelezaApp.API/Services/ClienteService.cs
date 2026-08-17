namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Cliente>> ObterTodos()
    {
        return await _repository.ObterTodos();
    }

    public async Task<Cliente?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<Cliente?> ObterPorEmail(string email)
    {
        return await _repository.ObterPorEmail(email);
    }

    public async Task<int> Criar(Cliente cliente)
    {
        var emailJaCadastrado = await _repository.ObterPorEmail(cliente.Email);
        if (emailJaCadastrado != null)
            throw new InvalidOperationException("Este email já está cadastrado.");

        cliente.Senha = Helpers.SenhaHelper.GerarHash(cliente.Senha);
        return await _repository.Criar(cliente);
    }

    public async Task Atualizar(Cliente cliente)
    {
        await _repository.Atualizar(cliente);
    }

    public async Task Deletar(int id)
    {
        await _repository.Deletar(id);
    }
}
