namespace BelezaApp.API.DTOs;

public class AgendaComNomesDTO
{
    public int Id { get; set; }
    public int? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public int UsuarioId { get; set; }
    public int ServicoId { get; set; }
    public string ServicoNome { get; set; } = string.Empty;
    public DateTime DataAgendamento { get; set; }
    public DateTime HoraInicio { get; set; }
    public DateTime HoraFim { get; set; }
    public bool Disponivel { get; set; }
    public DateTime CriadoEm { get; set; }
}
