namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly IDbConnection _connection;

    public AgendamentoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Agendamento>> ObterPorClienteId(int clienteId)
    {
        var sql = "SELECT * FROM Agendamento WHERE cliente_id = @ClienteId";
        return await _connection.QueryAsync<Agendamento>(sql, new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<Agendamento>> ObterPorFuncionarioId(int funcionarioId)
    {
        var sql = "SELECT * FROM Agendamento WHERE funcionario_id = @FuncionarioId";
        return await _connection.QueryAsync<Agendamento>(sql, new { FuncionarioId = funcionarioId });
    }

    public async Task<Agendamento?> ObterPorId(int id)
    {
        var sql = "SELECT * FROM Agendamento WHERE id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Agendamento>(sql, new { Id = id });
    }

    public async Task<int> Criar(Agendamento agendamento)
    {
        var sql = @"INSERT INTO Agendamento (agenda_id, cliente_id, servico_id, funcionario_id, usuario_id, data, horario_inicio, horario_fim, status)
                    VALUES (@AgendaId, @ClienteId, @ServicoId, @FuncionarioId, @UsuarioId, @Data, @HorarioInicio, @HorarioFim, @Status::status_agendamento_enum)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, agendamento);
    }

    public async Task AtualizarStatus(int id, string status)
    {
        var sql = "UPDATE Agendamento SET status = @Status::status_agendamento_enum, atualizado_em = NOW() WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id, Status = status });
    }
}
