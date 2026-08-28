namespace BelezaApp.API.DTOs;

public class ViaCepResponseDTO
{
    public string Logradouro { get; set; } = string.Empty;
    public string Bairro { get; set; } = string.Empty;
    public string Localidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public bool Erro { get; set; }
}
