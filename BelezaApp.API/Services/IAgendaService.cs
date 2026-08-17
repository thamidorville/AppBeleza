namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IAgendaService
{
    Task<IEnumerable<Agenda>> ObterPorFuncionarioId(int funcionarioId);
    Task<IEnumerable<Agenda>> ObterPorUsuarioId(int usuarioId);
    Task<Agenda?> ObterPorId(int id);
    Task<int> Criar(Agenda agenda);
    Task Atualizar(Agenda agenda);
    Task Deletar(int id);
}
