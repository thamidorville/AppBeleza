namespace BelezaApp.API.Models;

public class Agendamento
{
    public int Id { get; set; }
    public int AgendaId { get; set; }
    public int ClienteId { get; set; }
    public int ServicoId { get; set; }
    public int? FuncionarioId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Data { get; set; }
    public DateTime HorarioInicio { get; set; }
    public DateTime HorarioFim { get; set; }
    public string Status { get; set; } = "Confirmado";
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
