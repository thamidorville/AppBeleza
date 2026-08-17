namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IEnderecoService
{
    Task<Endereco?> ObterPorUsuarioId(int usuarioId, string emailUsuarioToken, string role);
    Task<int> Criar(Endereco endereco, string emailUsuarioToken);
    Task Atualizar(Endereco endereco);
    Task Deletar(int id);
}
