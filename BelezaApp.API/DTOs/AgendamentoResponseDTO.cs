namespace BelezaApp.API.DTOs;

public class AgendamentoResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string AgendaId { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public string? FuncionarioId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public DateTime HorarioInicio { get; set; }
    public DateTime HorarioFim { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
