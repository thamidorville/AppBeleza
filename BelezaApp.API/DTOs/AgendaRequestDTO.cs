namespace BelezaApp.API.DTOs;

public class AgendaRequestDTO
{
    public string? FuncionarioId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public DateTime DataAgendamento { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFim { get; set; }
}
