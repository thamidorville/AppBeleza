namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class ServicoService : IServicoService
{
    private readonly IServicoRepository _repository;

    public ServicoService(IServicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId)
    {
        return await _repository.ObterTodosPorUsuarioId(usuarioId);
    }

    public async Task<Servico?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Servico servico)
    {
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
