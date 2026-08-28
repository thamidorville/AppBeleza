namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IAuthService
{
    Task<int> CadastrarUsuario(Usuario usuario);
    Task<int> CadastrarCliente(Cliente cliente);
    Task<string> Login(string email, string senha);
    Task AlterarSenha(string email, string role, string senhaAtual, string novaSenha);
}
