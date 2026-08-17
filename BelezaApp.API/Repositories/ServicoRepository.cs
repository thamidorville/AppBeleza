namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class ServicoRepository : IServicoRepository
{
    private readonly IDbConnection _connection;

    public ServicoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Servico>> ObterTodosPorUsuarioId(int usuarioId)
    {
        var sql = "SELECT * FROM Servico WHERE usuario_id = @UsuarioId AND ativo = true";
        return await _connection.QueryAsync<Servico>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<Servico?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Servico WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Servico>(sql, new { Id = id });
    }

    public async Task<int> Criar(Servico servico)
    {
        var sql = @"INSERT INTO Servico (usuario_id, nome, descricao, preco, duracao_minutos)
                    VALUES (@UsuarioId, @Nome, @Descricao, @Preco, @DuracaoMinutos)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, servico);
    }

    public async Task Atualizar(Servico servico)
    {
        var sql = @"UPDATE Servico SET nome = @Nome, descricao = @Descricao, preco = @Preco,
                    duracao_minutos = @DuracaoMinutos, atualizado_em = NOW()
                    WHERE id = @Id";
        await _connection.ExecuteAsync(sql, servico);
    }

    public async Task Desativar(int id)
    {
        var sql = "UPDATE Servico SET ativo = false, atualizado_em = NOW() WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }
}
