namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.DTOs;
using BelezaApp.API.Models;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly IDbConnection _connection;

    public AgendamentoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorClienteId(int clienteId)
    {
        var sql = @"SELECT ag.id, ag.agenda_id, ag.cliente_id, ag.servico_id,
                           ag.funcionario_id, ag.usuario_id, ag.data,
                           ag.horario_inicio, ag.horario_fim, ag.status, ag.criado_em,
                           c.nome AS cliente_nome,
                           s.nome AS servico_nome,
                           f.nome AS funcionario_nome,
                           u.nome AS usuario_nome
                    FROM Agendamento ag
                    INNER JOIN Cliente c ON ag.cliente_id = c.id
                    INNER JOIN Servico s ON ag.servico_id = s.id
                    LEFT JOIN Funcionario f ON ag.funcionario_id = f.id
                    INNER JOIN Usuario u ON ag.usuario_id = u.id
                    WHERE ag.cliente_id = @ClienteId
                    ORDER BY ag.data DESC, ag.horario_inicio";
        return await _connection.QueryAsync<AgendamentoComNomesDTO>(sql, new { ClienteId = clienteId });
    }

    public async Task<IEnumerable<AgendamentoComNomesDTO>> ObterPorUsuarioId(int usuarioId)
    {
        var sql = @"SELECT ag.id, ag.agenda_id, ag.cliente_id, ag.servico_id,
                           ag.funcionario_id, ag.usuario_id, ag.data,
                           ag.horario_inicio, ag.horario_fim, ag.status, ag.criado_em,
                           c.nome AS cliente_nome,
                           s.nome AS servico_nome,
                           f.nome AS funcionario_nome,
                           u.nome AS usuario_nome
                    FROM Agendamento ag
                    INNER JOIN Cliente c ON ag.cliente_id = c.id
                    INNER JOIN Servico s ON ag.servico_id = s.id
                    LEFT JOIN Funcionario f ON ag.funcionario_id = f.id
                    INNER JOIN Usuario u ON ag.usuario_id = u.id
                    WHERE ag.usuario_id = @UsuarioId
                    ORDER BY ag.data DESC, ag.horario_inicio";
        return await _connection.QueryAsync<AgendamentoComNomesDTO>(sql, new { UsuarioId = usuarioId });
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
