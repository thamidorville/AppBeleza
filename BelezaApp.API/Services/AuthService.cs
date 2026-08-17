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

    public async Task<string> LoginUsuario(string email, string senha)
    {
        var usuario = await _usuarioRepository.ObterPorEmail(email);
        if (usuario == null || !SenhaHelper.Validar(senha, usuario.Senha))
            throw new InvalidOperationException("Email ou senha inválidos.");

        return TokenHelper.GerarToken(usuario.Id, usuario.Email, usuario.Role);
    }

    public async Task<string> LoginFuncionario(string email, string senha)
    {
        var funcionario = await _funcionarioRepository.ObterPorEmail(email);
        if (funcionario == null || !SenhaHelper.Validar(senha, funcionario.Senha))
            throw new InvalidOperationException("Email ou senha inválidos.");

        return TokenHelper.GerarToken(funcionario.Id, funcionario.Email, "Funcionario");
    }

    public async Task<string> LoginCliente(string email, string senha)
    {
        var cliente = await _clienteRepository.ObterPorEmail(email);
        if (cliente == null || !SenhaHelper.Validar(senha, cliente.Senha))
            throw new InvalidOperationException("Email ou senha inválidos.");

        return TokenHelper.GerarToken(cliente.Id, cliente.Email, "Cliente");
    }

    public async Task AlterarSenha(int id, string role, string senhaAtual, string novaSenha)
    {
        string? hashAtual = null;

        if (role == "Gestor" || role == "Admin")
        {
            var usuario = await _usuarioRepository.ObterPorId(id);
            if (usuario == null) throw new InvalidOperationException("Usuário não encontrado.");
            hashAtual = usuario.Senha;
        }
        else if (role == "Funcionario")
        {
            var funcionario = await _funcionarioRepository.ObterPorId(id);
            if (funcionario == null) throw new InvalidOperationException("Funcionário não encontrado.");
            hashAtual = funcionario.Senha;
        }
        else if (role == "Cliente")
        {
            var cliente = await _clienteRepository.ObterPorId(id);
            if (cliente == null) throw new InvalidOperationException("Cliente não encontrado.");
            hashAtual = cliente.Senha;
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
