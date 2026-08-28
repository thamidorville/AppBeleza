namespace BelezaApp.API.DTOs;

public class AgendaComNomesDTO
{
    public int Id { get; set; }
    public int? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public int ServicoId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public DateOnly DataAgendamento { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
