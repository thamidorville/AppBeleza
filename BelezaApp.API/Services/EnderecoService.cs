namespace BelezaApp.API.Services;

using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IGeocodingService _geocodingService;

    public EnderecoService(IEnderecoRepository repository, IUsuarioRepository usuarioRepository, IClienteRepository clienteRepository, IGeocodingService geocodingService)
    {
        _repository = repository;
        _usuarioRepository = usuarioRepository;
        _clienteRepository = clienteRepository;
        _geocodingService = geocodingService;
    }

    public async Task<Endereco?> ObterPorUsuarioId(int usuarioId, string emailUsuarioToken, string role)
    {
        if (role != "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
            if (usuario == null || usuario.Id != usuarioId)
                throw new UnauthorizedAccessException("Acesso negado.");
        }

        return await _repository.ObterPorUsuarioId(usuarioId);
    }

    public async Task<Endereco?> ObterPorClienteId(string emailClienteToken)
    {
        var cliente = await _clienteRepository.ObterPorEmail(emailClienteToken);
        if (cliente == null)
            throw new UnauthorizedAccessException("Cliente não encontrado.");

        return await _repository.ObterPorClienteId(cliente.Id);
    }

    public async Task<int> Criar(Endereco endereco, string emailUsuarioToken)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(emailUsuarioToken);
        if (usuario == null)
            throw new UnauthorizedAccessException("Usuário não encontrado.");

        endereco.UsuarioId = usuario.Id;

        var enderecoJaCadastrado = await _repository.ObterPorUsuarioId(usuario.Id);
        if (enderecoJaCadastrado != null)
            throw new InvalidOperationException("Usuário já possui endereço cadastrado.");

        var coordenadas = await _geocodingService.ObterCoordenadas(endereco.Logradouro, endereco.Cidade, endereco.Estado);
        if (coordenadas != null)
        {
            endereco.Latitude = coordenadas.Value.Latitude;
            endereco.Longitude = coordenadas.Value.Longitude;
        }

        return await _repository.Criar(endereco);
    }

    public async Task<int> CriarParaCliente(Endereco endereco, string emailClienteToken)
    {
        var cliente = await _clienteRepository.ObterPorEmail(emailClienteToken);
        if (cliente == null)
            throw new UnauthorizedAccessException("Cliente não encontrado.");

        endereco.ClienteId = cliente.Id;
        endereco.UsuarioId = null;

        var enderecoJaCadastrado = await _repository.ObterPorClienteId(cliente.Id);
        if (enderecoJaCadastrado != null)
            throw new InvalidOperationException("Cliente já possui endereço cadastrado.");

        var coordenadas = await _geocodingService.ObterCoordenadas(endereco.Logradouro, endereco.Cidade, endereco.Estado);
        if (coordenadas != null)
        {
            endereco.Latitude = coordenadas.Value.Latitude;
            endereco.Longitude = coordenadas.Value.Longitude;
        }

        return await _repository.Criar(endereco);
    }

    public async Task<int> CriarDireto(Endereco endereco)
    {
        var coordenadas = await _geocodingService.ObterCoordenadas(endereco.Logradouro, endereco.Cidade, endereco.Estado);
        if (coordenadas != null)
        {
            endereco.Latitude = coordenadas.Value.Latitude;
            endereco.Longitude = coordenadas.Value.Longitude;
        }

        return await _repository.Criar(endereco);
    }

    public async Task Atualizar(Endereco endereco)
    {
        var coordenadas = await _geocodingService.ObterCoordenadas(endereco.Logradouro, endereco.Cidade, endereco.Estado);
        if (coordenadas != null)
        {
            endereco.Latitude = coordenadas.Value.Latitude;
            endereco.Longitude = coordenadas.Value.Longitude;
        }

        await _repository.Atualizar(endereco);
    }

    public async Task Deletar(int id)
    {
        await _repository.Deletar(id);
    }
}
