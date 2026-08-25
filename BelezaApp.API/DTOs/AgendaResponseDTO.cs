namespace BelezaApp.API.DTOs;

public class AgendaResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string UsuarioNome { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public DateOnly DataAgendamento { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public bool Disponivel { get; set; }
    public DateTime CriadoEm { get; set; }
}
