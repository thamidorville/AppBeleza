namespace BelezaApp.API.DTOs;

public class AgendamentoComNomesDTO
{
    public int Id { get; set; }
    public int AgendaId { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public int ServicoId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public int? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public TimeOnly HorarioInicio { get; set; }
    public TimeOnly HorarioFim { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
