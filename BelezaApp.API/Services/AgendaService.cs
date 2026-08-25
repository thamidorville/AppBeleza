namespace BelezaApp.API.Services;

using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class AgendaService : IAgendaService
{
    private readonly IAgendaRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;

    public AgendaService(IAgendaRepository repository, IUsuarioRepository usuarioRepository)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<Agenda>> ObterPorFuncionarioId(int funcionarioId)
    {
        return await _repository.ObterPorFuncionarioId(funcionarioId);
    }

    public async Task<IEnumerable<AgendaComNomesDTO>> ObterPorUsuarioId(int usuarioId)
    {
        return await _repository.ObterPorUsuarioId(usuarioId);
    }

    public async Task<IEnumerable<AgendaComNomesDTO>> ObterMinhaAgenda(string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        return await _repository.ObterPorUsuarioId(usuario.Id);
    }

    public async Task<IEnumerable<AgendaComNomesDTO>> ObterVitrineDoCliente()
    {
        return await _repository.ObterVitrineDoCliente();
    }

    public async Task<Agenda?> ObterPorId(int id)
    {
        return await _repository.ObterPorId(id);
    }

    public async Task<int> Criar(Agenda agenda, string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        agenda.UsuarioId = usuario.Id;

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
