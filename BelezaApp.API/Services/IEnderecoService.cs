namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IEnderecoService
{
    Task<Endereco?> ObterPorUsuarioId(int usuarioId, string emailUsuarioToken, string role);
    Task<Endereco?> ObterPorClienteId(string emailClienteToken);
    Task<int> Criar(Endereco endereco, string emailUsuarioToken);
    Task<int> CriarParaCliente(Endereco endereco, string emailClienteToken);
    Task<int> CriarDireto(Endereco endereco);
    Task Atualizar(Endereco endereco);
    Task Deletar(int id);
}
