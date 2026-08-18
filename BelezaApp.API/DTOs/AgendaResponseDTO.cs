namespace BelezaApp.API.DTOs;

public class AgendaResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public DateTime DataAgendamento { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFim { get; set; }
    public bool Disponivel { get; set; }
    public DateTime CriadoEm { get; set; }
}
