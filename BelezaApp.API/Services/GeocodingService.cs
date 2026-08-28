namespace BelezaApp.API.Services;

using System.Globalization;
using System.Text.Json;

public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(decimal Latitude, decimal Longitude)?> ObterCoordenadas(string logradouro, string cidade, string estado)
    {
        var query = Uri.EscapeDataString($"{logradouro}, {cidade}, {estado}, Brazil");
        var url = $"https://nominatim.openstreetmap.org/search?q={query}&format=json&limit=1";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var resultados = JsonSerializer.Deserialize<JsonElement[]>(json);

        if (resultados == null || resultados.Length == 0)
            return null;

        var primeiro = resultados[0];
        var latStr = primeiro.GetProperty("lat").GetString();
        var lonStr = primeiro.GetProperty("lon").GetString();

        if (string.IsNullOrEmpty(latStr) || string.IsNullOrEmpty(lonStr))
            return null;

        var lat = decimal.Parse(latStr, CultureInfo.InvariantCulture);
        var lon = decimal.Parse(lonStr, CultureInfo.InvariantCulture);

        return (lat, lon);
    }
}
