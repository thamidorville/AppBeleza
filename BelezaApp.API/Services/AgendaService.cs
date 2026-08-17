namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class AgendaService : IAgendaService
{
    private readonly IAgendaRepository _repository;

    public AgendaService(IAgendaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Agenda>> ObterPorFuncionarioId(int funcionarioId)
    {
        return await _repository.ObterPorFuncionarioId(funcionarioId);
    }

    public async Task<IEnumerable<Agenda>> ObterPorUsuarioId(int usuarioId)
    {
        return await _repository.ObterPorUsuarioId(usuarioId);
    }

    public async Task<Agenda?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Agenda agenda)
    {
        if (agenda.UsuarioId == 0)
            throw new InvalidOperationException("A agenda deve estar vinculada a um usuário (gestor).");

        if (agenda.HoraFim <= agenda.HoraInicio)
            throw new InvalidOperationException("O horário de fim deve ser posterior ao horário de início.");

        return await _repository.Criar(agenda);
    }

    public async Task Atualizar(Agenda agenda)
    {
        await _repository.Atualizar(agenda);
    }

    public async Task Deletar(int id)
    {
        await _repository.Deletar(id);
    }
}
