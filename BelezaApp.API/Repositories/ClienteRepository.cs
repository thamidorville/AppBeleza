namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbConnection _connection;

    public ClienteRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Cliente>> ObterTodos()
    {
        var sql = "SELECT * FROM Cliente";
        return await _connection.QueryAsync<Cliente>(sql);
    }

    public async Task<Cliente?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Cliente WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Id = id });
    }

    public async Task<Cliente?> ObterPorEmail(string email)
    {
        var sql = "SELECT * FROM Cliente WHERE email = @Email";
        return await _connection.QueryFirstOrDefaultAsync<Cliente>(sql, new { Email = email });
    }

    public async Task<int> Criar(Cliente cliente)
    {
        var sql = @"INSERT INTO Cliente (nome, email, senha, telefone)
                    VALUES (@Nome, @Email, @Senha, @Telefone)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, cliente);
    }

    public async Task Atualizar(Cliente cliente)
    {
        var sql = "UPDATE Cliente SET nome = @Nome, email = @Email, telefone = @Telefone WHERE id = @Id";
        await _connection.ExecuteAsync(sql, cliente);
    }

    public async Task Deletar(int id)
    {
        var sql = "DELETE FROM Cliente WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task AtualizarSenha(int id, string senha)
    {
        var sql = "UPDATE Cliente SET senha = @Senha WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id, Senha = senha });
    }
}
