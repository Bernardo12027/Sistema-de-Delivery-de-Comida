namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Cliente da plataforma (único perfil do MVP). A senha nunca é guardada em texto puro.</summary>
public class Cliente
{
    public const int TamanhoMaximoNome = 100;
    public const int TamanhoMaximoEmail = 150;
    public const int TamanhoMaximoTelefone = 20;
    public const int TamanhoMaximoNomeUsuario = 30;
    public const int TamanhoMaximoSenhaHash = 256;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string NomeUsuario { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public DateTime DataCadastro { get; private set; }

    public ICollection<Endereco> Enderecos { get; private set; } = new List<Endereco>();
    public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();
    public ICollection<CartaoSimulado> Cartoes { get; private set; } = new List<CartaoSimulado>();
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

    protected Cliente()
    {
    }

    public Cliente(string nome, string email, string telefone, string nomeUsuario, string senhaHash, DateTime dataCadastro)
    {
        Nome = Guarda.TextoObrigatorio(nome, "Nome", TamanhoMaximoNome);
        Email = NormalizarEmail(email);
        Telefone = Guarda.TextoOpcional(telefone, "Telefone", TamanhoMaximoTelefone) ?? string.Empty;
        NomeUsuario = NormalizarNomeUsuario(nomeUsuario);
        SenhaHash = Guarda.TextoObrigatorio(senhaHash, "Senha", TamanhoMaximoSenhaHash);
        DataCadastro = dataCadastro;
    }

    /// <summary>NOVA FUNCIONALIDADE: Editar perfil (nome e telefone).</summary>
    public void AtualizarPerfil(string nome, string telefone)
    {
        Nome = Guarda.TextoObrigatorio(nome, "Nome", TamanhoMaximoNome);
        Telefone = Guarda.TextoOpcional(telefone, "Telefone", TamanhoMaximoTelefone) ?? string.Empty;
    }

    public static string NormalizarEmail(string email) =>
        Guarda.TextoObrigatorio(email, "E-mail", TamanhoMaximoEmail).ToLowerInvariant();

    public static string NormalizarNomeUsuario(string nomeUsuario) =>
        Guarda.TextoObrigatorio(nomeUsuario, "Nome de usuário", TamanhoMaximoNomeUsuario).ToLowerInvariant();
}
