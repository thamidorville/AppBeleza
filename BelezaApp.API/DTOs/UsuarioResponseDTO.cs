namespace BelezaApp.API.DTOs;

public class UsuarioResponseDTO
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TipoPessoa { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
