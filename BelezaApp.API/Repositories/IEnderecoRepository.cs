namespace BelezaApp.API.Repositories;

using BelezaApp.API.Models;

public interface IEnderecoRepository
{
    Task<Endereco?> ObterPorUsuarioId(int usuarioId);
    Task<Endereco?> ObterPorClienteId(int clienteId);
    Task<int> Criar(Endereco endereco);
    Task Atualizar(Endereco endereco);
    Task Deletar(int id);
}
