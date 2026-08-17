namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;

    public EnderecoService(IEnderecoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Endereco?> ObterPorUsuarioId(int usuarioId)
    {
        return await _repository.ObterPorUsuarioId(usuarioId);
    }

    public async Task<int> Criar(Endereco endereco)
    {
        var enderecoJaCadastrado = await _repository.ObterPorUsuarioId(endereco.UsuarioId);
        if (enderecoJaCadastrado != null)
            throw new InvalidOperationException("Usuário já possui endereço cadastrado.");

        return await _repository.Criar(endereco);
    }

    public async Task Atualizar(Endereco endereco)
    {
        await _repository.Atualizar(endereco);
    }

    public async Task Deletar(int id)
    {
        await _repository.Deletar(id);
    }
}
