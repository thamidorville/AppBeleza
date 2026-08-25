namespace BelezaApp.API.DTOs;

public class AgendaRequestDTO
{
    public string? FuncionarioId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public DateOnly DataAgendamento { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
}
