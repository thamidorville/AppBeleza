namespace BelezaApp.API.Services;

using BelezaApp.API.DTOs;
using BelezaApp.API.Models;

public interface IAgendamentoService
{
    Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorClienteId(int clienteId);
    Task<IEnumerable<AgendamentoComNomesDTO>> ObterMeusAgendamentos(string emailClienteToken);
    Task<IEnumerable<AgendamentoComNomesDTO>> ObterMeusAgendamentosGestor(string emailGestorToken);
    Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId);
    Task<Agendamento?> ObterPorId(int id);
    Task<int> Criar(int agendaId, string emailClienteToken);
    Task CancelarMeuAgendamento(int agendamentoId, string emailClienteToken, string motivo);
    Task CancelarComoGestor(int agendamentoId, string emailGestorToken, string motivo);
    Task AtualizarStatus(int id, string status);
}
