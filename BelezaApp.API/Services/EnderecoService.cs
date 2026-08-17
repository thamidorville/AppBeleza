namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public EnderecoService(IEnderecoRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Endereco?> ObterPorUsuarioId(int usuarioId, string emailUsuarioToken, string role)
    {
        if (role != "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
            if (usuario == null || usuario.Id != usuarioId)
                throw new UnauthorizedAccessException("Acesso negado.");
        }

        return await _repository.ObterPorUsuarioId(usuarioId);
    }

    public async Task<int> Criar(Endereco endereco, string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        endereco.UsuarioId = usuario.Id;

        var enderecoJaCadastrado = await _repository.ObterPorUsuarioId(usuario.Id);
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
