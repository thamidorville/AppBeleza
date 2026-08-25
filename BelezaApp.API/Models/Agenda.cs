namespace BelezaApp.API.Models;

public class Agenda
{
    public int Id { get; set; }
    public int? FuncionarioId { get; set; }
    public int UsuarioId { get; set; }
    public int ServicoId { get; set; }
    public DateOnly DataAgendamento { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public bool Disponivel { get; set; } = true;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
