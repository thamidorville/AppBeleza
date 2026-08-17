namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly IDbConnection _connection;

    public FuncionarioRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Funcionario>> ObterTodosPorUsuarioId(int usuarioId)
    {
        var sql = "SELECT * FROM Funcionario WHERE usuario_id = @UsuarioId AND ativo = true";
        return await _connection.QueryAsync<Funcionario>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<Funcionario?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Funcionario WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Funcionario>(sql, new { Id = id });
    }

    public async Task<Funcionario?> ObterPorEmail(string email)
    {
        var sql = "SELECT * FROM Funcionario WHERE email = @Email";
        return await _connection.QueryFirstOrDefaultAsync<Funcionario>(sql, new { Email = email });
    }

    public async Task<int> Criar(Funcionario funcionario)
    {
        var sql = @"INSERT INTO Funcionario (usuario_id, nome, email, senha, telefone)
                    VALUES (@UsuarioId, @Nome, @Email, @Senha, @Telefone)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, funcionario);
    }

    public async Task Atualizar(Funcionario funcionario)
    {
        var sql = @"UPDATE Funcionario SET nome = @Nome, email = @Email, telefone = @Telefone, atualizado_em = NOW()
                    WHERE id = @Id";
        await _connection.ExecuteAsync(sql, funcionario);
    }

    public async Task Desativar(int id)
    {
        var sql = "UPDATE Funcionario SET ativo = false, atualizado_em = NOW() WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }

    public async Task AtualizarSenha(int id, string senha)
    {
        var sql = "UPDATE Funcionario SET senha = @Senha, atualizado_em = NOW() WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id, Senha = senha });
    }
}
