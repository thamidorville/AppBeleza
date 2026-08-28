namespace BelezaApp.API.Services;

public interface IGeocodingService
{
    Task<(decimal Latitude, decimal Longitude)?> ObterCoordenadas(string logradouro, string cidade, string estado);
}
