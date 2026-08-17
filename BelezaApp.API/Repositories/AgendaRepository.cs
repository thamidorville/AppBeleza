namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class AgendaRepository : IAgendaRepository
{
    private readonly IDbConnection _connection;

    public AgendaRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Agenda>> ObterPorFuncionarioId(int funcionarioId)
    {
        var sql = "SELECT * FROM Agenda WHERE funcionario_id = @FuncionarioId AND disponivel = true";
        return await _connection.QueryAsync<Agenda>(sql, new { FuncionarioId = funcionarioId });
    }

    public async Task<IEnumerable<Agenda>> ObterPorUsuarioId(int usuarioId)
    {
        var sql = "SELECT * FROM Agenda WHERE usuario_id = @UsuarioId AND disponivel = true";
        return await _connection.QueryAsync<Agenda>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<Agenda?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Agenda WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Agenda>(sql, new { Id = id });
    }

    public async Task<int> Criar(Agenda agenda)
    {
        var sql = @"INSERT INTO Agenda (funcionario_id, usuario_id, servico_id, data_agendamento, hora_inicio, hora_fim)
                    VALUES (@FuncionarioId, @UsuarioId, @ServicoId, @DataAgendamento, @HoraInicio, @HoraFim)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, agenda);
    }

    public async Task Atualizar(Agenda agenda)
    {
        var sql = @"UPDATE Agenda SET servico_id = @ServicoId, data_agendamento = @DataAgendamento,
                    hora_inicio = @HoraInicio, hora_fim = @HoraFim, disponivel = @Disponivel, atualizado_em = NOW()
                    WHERE id = @Id";
        await _connection.ExecuteAsync(sql, agenda);
    }

    public async Task Deletar(int id)
    {
        var sql = "DELETE FROM Agenda WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }
}
