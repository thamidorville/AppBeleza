namespace BelezaApp.API.Repositories;

using BelezaApp.API.DTOs;
using BelezaApp.API.Models;

public interface IAgendamentoRepository
{
    Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorClienteId(int clienteId);
    Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorUsuarioId(int usuarioId);
    Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId);
    Task<Agendamento?> ObterPorId(int id);
    Task<int> Criar(Agendamento agendamento);
    Task AtualizarStatus(int id, string status);
}
