namespace BelezaApp.API.Services;

using BelezaApp.API.DTOs;
using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class AgendamentoService : IAgendamentoService
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly IAgendaRepository _agendaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public AgendamentoService(
        IAgendamentoRepository agendamentoRepository,
        IAgendaRepository agendaRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository)
    {
        _agendamentoRepository = agendamentoRepository;
        _agendaRepository = agendaRepository;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorClienteId(int clienteId)
    {
        return await _agendamentoRepository.ObterPorClienteId(clienteId);
    }

    public async Task<IEnumerable<AgendamentoComNomesDTO>> ObterMeusAgendamentos(string emailClienteToken)
    {
        var cliente = await _clienteRepository.ObterPorEmail(emailClienteToken);
        if (cliente == null)
            throw new UnauthorizedAccessException("Cliente não encontrado.");

        return await _agendamentoRepository.ObterPorClienteId(cliente.Id);
    }

    public async Task<IEnumerable<AgendamentoComNomesDTO>> ObterMeusAgendamentosGestor(string emailGestorToken)
    {
        var gestor = await _usuarioRepository.ObterPorEmail(emailGestorToken);
        if (gestor == null)
            throw new UnauthorizedAccessException("Gestor não encontrado.");

        return await _agendamentoRepository.ObterPorUsuarioId(gestor.Id);
    }

    public async Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId)
    {
        return await _agendamentoRepository.ObterPorFuncionarioId(funcionarioId);
    }

    public async Task<Agendamento?> ObterPorId(int id)
    {
        return await _agendamentoRepository.ObterPorId(id);
    }

    public async Task<int> Criar(int agendaId, string emailClienteToken)
    {
        var cliente = await _clienteRepository.ObterPorEmail(emailClienteToken);
        if (cliente == null)
            throw new UnauthorizedAccessException("Cliente não encontrado.");

        var agenda = await _agendaRepository.ObterPorId(agendaId);
        if (agenda == null)
            throw new InvalidOperationException("Agenda não encontrada.");

        var existente = await _agendamentoRepository.ObterPorClienteEAgenda(cliente.Id, agendaId);
        if (existente != null && existente.Status == "Cancelado")
        {
            await _agendamentoRepository.AtualizarStatus(existente.Id, "Confirmado");

            agenda.Status = "Ocupado";
            await _agendaRepository.Atualizar(agenda);

            return existente.Id;
        }

        if (agenda.Status != "Disponivel")
            throw new InvalidOperationException("Este horário já foi agendado por outro cliente.");

        var agendamento = new Agendamento
        {
            AgendaId = agendaId,
            ClienteId = cliente.Id,
            ServicoId = agenda.ServicoId,
            FuncionarioId = agenda.FuncionarioId,
            UsuarioId = agenda.UsuarioId,
            Data = agenda.DataAgendamento,
            HorarioInicio = agenda.HoraInicio,
            HorarioFim = agenda.HoraFim
        };

        var id = await _agendamentoRepository.Criar(agendamento);

        agenda.Status = "Ocupado";
        await _agendaRepository.Atualizar(agenda);

        return id;
    }

    public async Task CancelarMeuAgendamento(int agendamentoId, string emailClienteToken, string motivo)
    {
        var cliente = await _clienteRepository.ObterPorEmail(emailClienteToken);
        if (cliente == null)
            throw new UnauthorizedAccessException("Cliente não encontrado.");

        var agendamento = await _agendamentoRepository.ObterPorId(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado.");

        if (agendamento.ClienteId != cliente.Id)
            throw new UnauthorizedAccessException("Este agendamento não pertence a você.");

        if (agendamento.Status != "Confirmado")
            throw new InvalidOperationException("Só é possível cancelar agendamentos confirmados.");

        await _agendamentoRepository.Cancelar(agendamentoId, motivo, "Cliente");

        var agenda = await _agendaRepository.ObterPorId(agendamento.AgendaId);
        if (agenda != null)
        {
            agenda.Status = "Disponivel";
            await _agendaRepository.Atualizar(agenda);
        }
    }

    public async Task CancelarComoGestor(int agendamentoId, string emailGestorToken, string motivo)
    {
        var gestor = await _usuarioRepository.ObterPorEmail(emailGestorToken);
        if (gestor == null)
            throw new UnauthorizedAccessException("Gestor não encontrado.");

        var agendamento = await _agendamentoRepository.ObterPorId(agendamentoId);
        if (agendamento == null)
            throw new InvalidOperationException("Agendamento não encontrado.");

        if (agendamento.UsuarioId != gestor.Id)
            throw new UnauthorizedAccessException("Este agendamento não pertence ao seu salão.");

        if (agendamento.Status != "Confirmado")
            throw new InvalidOperationException("Só é possível cancelar agendamentos confirmados.");

        await _agendamentoRepository.Cancelar(agendamentoId, motivo, "Gestor");

        var agenda = await _agendaRepository.ObterPorId(agendamento.AgendaId);
        if (agenda != null)
        {
            agenda.Status = "Cancelado";
            await _agendaRepository.Atualizar(agenda);
        }
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
                    agenda.Status = "Disponivel";
                    await _agendaRepository.Atualizar(agenda);
                }
            }
        }
    }
}
