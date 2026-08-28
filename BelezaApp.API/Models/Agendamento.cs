namespace BelezaApp.API.Models;

public class Agendamento
{
    public int Id { get; set; }
    public int AgendaId { get; set; }
    public int ClienteId { get; set; }
    public int ServicoId { get; set; }
    public int? FuncionarioId { get; set; }
    public int UsuarioId { get; set; }
    public DateOnly Data { get; set; }
    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFim { get; set; }
    public string Status { get; set; } = "Confirmado";
    public string? MotivoCancelamento { get; set; }
    public string? CanceladoPor { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
