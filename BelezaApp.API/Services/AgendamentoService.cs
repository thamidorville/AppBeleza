namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IAgendaRepository _agendaRepository;

    public AgendamentoService(IAgendamentoRepository agendamentoRepository, IAgendaRepository agendaRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _agendaRepository = agendaRepository;
    }

    public async Task<IEnumerable<Agendamento>> ObterPorClienteId(int clienteId)
    {
        return await _agendamentoRepository.ObterPorClienteId(clienteId);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId)
    {
        return await _agendamentoRepository.ObterPorFuncionarioId(funcionarioId);
    }

    public async Task<Agendamento?> ObterPorId(int id)
    {
        return await _agendamentoRepository.ObterPorId(id);
    }

    public async Task<int> Criar(Agendamento agendamento)
    {
        var agenda = await _agendaRepository.ObterPorId(agendamento.AgendaId);

        if (agenda == null)
            throw new InvalidOperationException("Agenda não encontrada.");

        if (!agenda.Disponivel)
            throw new InvalidOperationException("Este horário já foi agendado por outro cliente.");

        var id = await _agendamentoRepository.Criar(agendamento);

        agenda.Disponivel = false;
        await _agendaRepository.Atualizar(agenda);

        return id;
    }

    public async Task AtualizarStatus(int id, string status)
    {
        var statusValidos = new[] { "Confirmado", "Cancelado", "Concluido" };
        if (!statusValidos.Contains(status))
            throw new InvalidOperationException("Status inválido. Use: Confirmado, Cancelado ou Concluido.");

        await _agendamentoRepository.AtualizarStatus(id, status);

        if (status == "Cancelado")
        {
            var agendamento = await _agendamentoRepository.ObterPorId(id);
            if (agendamento != null)
            {
                var agenda = await _agendaRepository.ObterPorId(agendamento.AgendaId);
                if (agenda != null)
                {
                    agenda.Disponivel = true;
                    await _agendaRepository.Atualizar(agenda);
                }
            }
        }
    }
}
