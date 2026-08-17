namespace BelezaApp.API.Services;

using BelezaApp.API.Helpers;
using BelezaApp.API.Models;
using BelezaApp.API.Repositories;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;
    private readonly IClienteRepository _clienteRepository;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IFuncionarioRepository funcionarioRepository,
        IClienteRepository clienteRepository)
    {
        _usuarioRepository = usuarioRepository;
        _funcionarioRepository = funcionarioRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task CadastrarUsuario(Usuario usuario)
    {
        usuario.Senha = SenhaHelper.GerarHash(usuario.Senha);
        usuario.Role = "Gestor";
        await _usuarioRepository.Criar(usuario);
    }

    public async Task CadastrarCliente(Cliente cliente)
    {
        cliente.Senha = SenhaHelper.GerarHash(cliente.Senha);
        await _clienteRepository.Criar(cliente);
    }

    public async Task<string> Login(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);
        if (usuario != null && SenhaHelper.Validar(senha, usuario.Senha))
            return TokenHelper.GerarToken(usuario.Email, usuario.Role, usuario.Nome);

        var funcionario = await _funcionarioRepository.ObterPorEmail(email);
        if (funcionario != null && SenhaHelper.Validar(senha, funcionario.Senha))
            return TokenHelper.GerarToken(funcionario.Email, "Funcionario", funcionario.Nome);

        var cliente = await _clienteRepository.ObterPorEmail(email);
        if (cliente != null && SenhaHelper.Validar(senha, cliente.Senha))
            return TokenHelper.GerarToken(cliente.Email, "Cliente", cliente.Nome);

        throw new InvalidOperationException("Email ou senha inválidos.");
    }

    public async Task AlterarSenha(string email, string role, string senhaAtual, string novaSenha)
    {
        string? hashAtual = null;
        int id = 0;

        if (role == "Gestor" || role == "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorEmail(email);
            if (usuario == null) throw new InvalidOperationException("Usuário não encontrado.");
            hashAtual = usuario.Senha;
            id = usuario.Id;
        }
        else if (role == "Funcionario")
        {
            var funcionario = await _funcionarioRepository.ObterPorEmail(email);
            if (funcionario == null) throw new InvalidOperationException("Funcionário não encontrado.");
            hashAtual = funcionario.Senha;
            id = funcionario.Id;
        }
        else if (role == "Cliente")
        {
            var cliente = await _clienteRepository.ObterPorEmail(email);
            if (cliente == null) throw new InvalidOperationException("Cliente não encontrado.");
            hashAtual = cliente.Senha;
            id = cliente.Id;
        }
        else
        {
            throw new InvalidOperationException("Role inválida.");
        }

        if (!SenhaHelper.Validar(senhaAtual, hashAtual!))
            throw new InvalidOperationException("Senha atual incorreta.");

        var novoHash = SenhaHelper.GerarHash(novaSenha);

        if (role == "Gestor" || role == "Admin")
            await _usuarioRepository.AtualizarSenha(id, novoHash);
        else if (role == "Funcionario")
            await _funcionarioRepository.AtualizarSenha(id, novoHash);
        else if (role == "Cliente")
            await _clienteRepository.AtualizarSenha(id, novoHash);
    }
}
