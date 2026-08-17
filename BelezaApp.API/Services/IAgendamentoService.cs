namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IAgendamentoService
{
    Task<IEnumerable<Agendamento>> ObterPorClienteId(int clienteId);
    Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId);
    Task<Agendamento?> ObterPorId(int id);
    Task<int> Criar(Agendamento agendamento);
    Task AtualizarStatus(int id, string status);
}
