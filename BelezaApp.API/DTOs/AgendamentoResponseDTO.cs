namespace BelezaApp.API.DTOs;

public class AgendamentoResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string AgendaId { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public string ServicoId { get; set; } = string.Empty;
    public string ServicoNome { get; set; } = string.Empty;
    public decimal ServicoPreco { get; set; }
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string UsuarioNome { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFim { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MotivoCancelamento { get; set; }
    public string? CanceladoPor { get; set; }
    public DateTime CriadoEm { get; set; }
}
