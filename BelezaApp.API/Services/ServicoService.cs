namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ServicoService(IServicoRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<Servico>> ObterMeusServicos(string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        return await _repository.ObterTodosPorUsuarioId(usuario.Id);
    }

    public async Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId, string emailUsuarioToken, string role)
    {
        if (role != "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
            if (usuario == null || usuario.Id != usuarioId)
                throw new UnauthorizedAccessException("Acesso negado.");
        }

        return await _repository.ObterTodosPorUsuarioId(usuarioId);
    }

    public async Task<Servico?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Servico servico, string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        servico.UsuarioId = usuario.Id;
        return await _repository.Criar(servico);
    }

    public async Task Atualizar(Servico servico)
    {
        await _repository.Atualizar(servico);
    }

    public async Task Desativar(int id)
    {
        await _repository.Desativar(id);
    }
}
