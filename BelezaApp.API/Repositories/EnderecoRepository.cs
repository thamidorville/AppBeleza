namespace BelezaApp.API.Repositories;

using System.Data;
using Dapper;
using BelezaApp.API.Models;

public class EnderecoRepository : IEnderecoRepository
{
    private readonly IDbConnection _connection;

    public EnderecoRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Endereco?> ObterPorUsuarioId(int usuarioId)
    {
        var sql = "SELECT * FROM Endereco WHERE usuario_id = @UsuarioId";
        return await _connection.QueryFirstOrDefaultAsync<Endereco>(sql, new { UsuarioId = usuarioId });
    }

    public async Task<Endereco?> ObterPorClienteId(int clienteId)
    {
        var sql = "SELECT * FROM Endereco WHERE cliente_id = @ClienteId";
        return await _connection.QueryFirstOrDefaultAsync<Endereco>(sql, new { ClienteId = clienteId });
    }

    public async Task<int> Criar(Endereco endereco)
    {
        var sql = @"INSERT INTO Endereco (usuario_id, cliente_id, logradouro, numero, complemento, bairro, cidade, estado, cep, latitude, longitude)
                    VALUES (@UsuarioId, @ClienteId, @Logradouro, @Numero, @Complemento, @Bairro, @Cidade, @Estado, @Cep, @Latitude, @Longitude)
                    RETURNING id";
        return await _connection.ExecuteScalarAsync<int>(sql, endereco);
    }

    public async Task Atualizar(Endereco endereco)
    {
        var sql = @"UPDATE Endereco SET logradouro = @Logradouro, numero = @Numero, complemento = @Complemento,
                    bairro = @Bairro, cidade = @Cidade, estado = @Estado, cep = @Cep,
                    latitude = @Latitude, longitude = @Longitude, atualizado_em = NOW()
                    WHERE id = @Id";
        await _connection.ExecuteAsync(sql, endereco);
    }

    public async Task Deletar(int id)
    {
        var sql = "DELETE FROM Endereco WHERE id = @Id";
        await _connection.ExecuteAsync(sql, new { Id = id });
    }
}
