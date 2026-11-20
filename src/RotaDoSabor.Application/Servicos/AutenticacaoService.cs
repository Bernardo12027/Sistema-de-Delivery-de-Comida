namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Seguranca;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Application.Validacoes;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Excecoes;

public interface IAutenticacaoService
{
    Task<int> CadastrarAsync(CadastroClienteDto dados);

    /// <summary>Login por e-mail OU nome de usuário. Em caso de sucesso inicia a sessão.</summary>
    Task EntrarAsync(string emailOuUsuario, string senha);

    void Sair();
}

public sealed class AutenticacaoService : IAutenticacaoService
{
    private const string MensagemCredenciaisInvalidas = "Usuário/e-mail ou senha incorretos.";

    private readonly IClienteRepository _clientes;
    private readonly IPasswordHasher _hasher;
    private readonly ISessaoUsuario _sessao;
    private readonly IRelogio _relogio;

    public AutenticacaoService(IClienteRepository clientes, IPasswordHasher hasher, ISessaoUsuario sessao, IRelogio relogio)
    {
        _clientes = clientes;
        _hasher = hasher;
        _sessao = sessao;
        _relogio = relogio;
    }

    public async Task<int> CadastrarAsync(CadastroClienteDto dados)
    {
        ValidacaoException.LancarSeHouverErros(ValidadorCadastroCliente.Validar(dados));

        var email = Cliente.NormalizarEmail(dados.Email);
        var nomeUsuario = Cliente.NormalizarNomeUsuario(dados.NomeUsuario);

        if (await _clientes.ExisteEmailAsync(email))
            throw new RegraDeNegocioException("Já existe uma conta com este e-mail.");
        if (await _clientes.ExisteNomeUsuarioAsync(nomeUsuario))
            throw new RegraDeNegocioException("Este nome de usuário já está em uso.");

        var cliente = new Cliente(dados.Nome, email, RegrasValidacao.ApenasDigitos(dados.Telefone), nomeUsuario,
            _hasher.GerarHash(dados.Senha), _relogio.Agora);

        await _clientes.AdicionarAsync(cliente);
        return cliente.Id;
    }

    public async Task EntrarAsync(string emailOuUsuario, string senha)
    {
        if (RegrasValidacao.Vazio(emailOuUsuario) || string.IsNullOrEmpty(senha))
            throw new ValidacaoException(["Informe usuário (ou e-mail) e senha."]);

        var login = emailOuUsuario.Trim().ToLowerInvariant();
        var cliente = login.Contains('@')
            ? await _clientes.ObterPorEmailAsync(login)
            : await _clientes.ObterPorNomeUsuarioAsync(login);

        // Mesma mensagem para usuário inexistente e senha errada (não revela quais contas existem).
        if (cliente is null || !_hasher.Verificar(senha, cliente.SenhaHash))
            throw new RegraDeNegocioException(MensagemCredenciaisInvalidas);

        _sessao.Iniciar(cliente.Id, cliente.Nome);
    }

    public void Sair() => _sessao.Encerrar();
}
