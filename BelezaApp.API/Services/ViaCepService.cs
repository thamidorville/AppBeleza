namespace BelezaApp.API.Services;

using System.Text.Json;
using BelezaApp.API.DTOs;

public class ViaCepService : IViaCepService
{
    private readonly HttpClient _httpClient;

    public ViaCepService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ViaCepResponseDTO?> ConsultarCep(string cep)
    {
        var cepLimpo = cep.Replace("-", "").Replace(".", "").Trim();

        var response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cepLimpo}/json/");

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var resultado = JsonSerializer.Deserialize<ViaCepResponseDTO>(json, options);

        if (resultado == null || resultado.Erro)
            return null;

        return resultado;
    }
}
