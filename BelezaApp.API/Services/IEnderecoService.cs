namespace BelezaApp.API.Services;

using BelezaApp.API.Models;

public interface IEnderecoService
{
    Task<Endereco?> ObterPorUsuarioId(int usuarioId);
    Task<int> Criar(Endereco endereco);
    Task Atualizar(Endereco endereco);
    Task Deletar(int id);
}
