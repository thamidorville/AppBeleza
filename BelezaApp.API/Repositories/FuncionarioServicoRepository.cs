namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class FuncionarioServicoRepository : IFuncionarioServicoRepository
{
    private readonly IDbConnection _connection;

    public FuncionarioServicoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Servico>> ObterServicosPorFuncionarioId(int funcionarioId)
    {
        var sql = @"SELECT s.* FROM Servico s
                    INNER JOIN FuncionarioServico fs ON fs.servico_id = s.id
                    WHERE fs.funcionario_id = @FuncionarioId AND s.ativo = true";
        return await _connection.QueryAsync<Servico>(sql, new { FuncionarioId = funcionarioId });
    }

    public async Task<IEnumerable<Funcionario>> ObterFuncionariosPorServicoId(int servicoId)
    {
        var sql = @"SELECT f.* FROM Funcionario f
                    INNER JOIN FuncionarioServico fs ON fs.funcionario_id = f.id
                    WHERE fs.servico_id = @ServicoId AND f.ativo = true";
        return await _connection.QueryAsync<Funcionario>(sql, new { ServicoId = servicoId });
    }

    public async Task<int> Vincular(FuncionarioServico funcionarioServico)
    {
        var sql = @"INSERT INTO FuncionarioServico (funcionario_id, servico_id)
                    VALUES (@FuncionarioId, @ServicoId)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, funcionarioServico);
    }

    public async Task<bool> VinculoJaExiste(int funcionarioId, int servicoId)
    {
        var sql = "SELECT COUNT(1) FROM FuncionarioServico WHERE funcionario_id = @FuncionarioId AND servico_id = @ServicoId";
        var count = await _connection.ExecuteScalarAsync<int>(sql, new { FuncionarioId = funcionarioId, ServicoId = servicoId });
        return count > 0;
    }

    public async Task Desvincular(int funcionarioId, int servicoId)
    {
        var sql = "DELETE FROM FuncionarioServico WHERE funcionario_id = @FuncionarioId AND servico_id = @ServicoId";
        await _connection.ExecuteAsync(sql, new { FuncionarioId = funcionarioId, ServicoId = servicoId });
    }
}
