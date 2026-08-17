namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnection _connection;

    public UsuarioRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Usuario>> ObterTodos()
    {
        var sql = "SELECT * FROM Usuario";
        return await _connection.QueryAsync<Usuario>(sql);
    }

    public async Task<Usuario?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Usuario WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Id = id });
    }

    public async Task<Usuario?> ObterPorEmail(string email)
    {
        var sql = "SELECT * FROM Usuario WHERE email = @Email";
        return await _connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { Email = email });
    }

    public async Task<int> Criar(Usuario usuario)
    {
        var sql = @"INSERT INTO Usuario (nome, email, senha, tipo_pessoa, documento, telefone, role)
                    VALUES (@Nome, @Email, @Senha, @TipoPessoa::tipo_pessoa_enum, @Documento, @Telefone, @Role::role_enum)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, usuario);
    }

    public async Task Atualizar(Usuario usuario)
    {
        var sql = @"UPDATE Usuario SET nome = @Nome, email = @Email, telefone = @Telefone, atualizado_em = NOW()
                    WHERE id = @Id";
        await _connection.ExecuteAsync(sql, usuario);
    }

    public async Task Deletar(int id)
    {
        var sql = "DELETE FROM Usuario WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task AtualizarSenha(int id, string senha)
    {
        var sql = "UPDATE Usuario SET senha = @Senha, atualizado_em = NOW() WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id, Senha = senha });
    }
}
