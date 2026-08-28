namespace BelezaApp.API.Services;

using BelezaApp.API.DTOs;

public interface IViaCepService
{
    Task<ViaCepResponseDTO?> ConsultarCep(string cep);
}
