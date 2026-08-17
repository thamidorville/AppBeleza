namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IAuthService
{
    Task CadastrarUsuario(Usuario usuario);
    Task CadastrarCliente(Cliente cliente);
    Task<string> LoginUsuario(string email, string senha);
    Task<string> LoginFuncionario(string email, string senha);
    Task<string> LoginCliente(string email, string senha);
    Task AlterarSenha(int id, string role, string senhaAtual, string novaSenha);
}
