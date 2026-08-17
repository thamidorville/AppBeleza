namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IAuthService
{
    Task CadastrarUsuario(Usuario usuario);
    Task CadastrarCliente(Cliente cliente);
    Task<string> Login(string email, string senha);
    Task AlterarSenha(string email, string role, string senhaAtual, string novaSenha);
}
