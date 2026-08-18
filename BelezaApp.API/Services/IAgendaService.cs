namespace BelezaApp.API.Services;

using BelezaApp.API.DTOs;
using BelezaApp.API.Models;

public interface IAgendaService
{
    Task<IEnumerable<Agenda>> ObterPorFuncionarioId(int funcionarioId);
    Task<IEnumerable<AgendaComNomesDTO>> ObterPorUsuarioId(int usuarioId);
    Task<IEnumerable<AgendaComNomesDTO>> ObterMinhaAgenda(string emailUsuarioToken);
    Task<Agenda?> ObterPorId(int id);
    Task<int> Criar(Agenda agenda, string emailUsuarioToken);
    Task Atualizar(Agenda agenda);
    Task Deletar(int id);
}
