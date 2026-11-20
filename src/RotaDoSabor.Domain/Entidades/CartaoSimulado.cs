namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// NOVA FUNCIONALIDADE: cartão fictício do cliente.
/// Por segurança guarda apenas: identificador fictício, bandeira, últimos 4 dígitos, tipo e apelido.
/// O número completo, nome impresso, validade e CVV NUNCA são armazenados.
/// </summary>
public class CartaoSimulado
{
    public const int TamanhoMaximoIdentificador = 40;
    public const int TamanhoMaximoBandeira = 20;
    public const int TamanhoMaximoApelido = 40;
    public const int QuantidadeUltimosDigitos = 4;

    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    public string IdentificadorFicticio { get; private set; } = string.Empty;
    public string Bandeira { get; private set; } = string.Empty;
    public string UltimosDigitos { get; private set; } = string.Empty;
    public TipoCartao Tipo { get; private set; }
    public string Apelido { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime DataCadastro { get; private set; }

    public ICollection<Pagamento> Pagamentos { get; private set; } = new List<Pagamento>();

    protected CartaoSimulado()
    {
    }

    private CartaoSimulado(int clienteId, string bandeira, string ultimosDigitos, TipoCartao tipo, string apelido, DateTime dataCadastro)
    {
        ClienteId = Guarda.IdValido(clienteId, "Cliente");
        IdentificadorFicticio = $"SIM-{Guid.NewGuid():N}"[..24].ToUpperInvariant();
        Bandeira = bandeira;
        UltimosDigitos = ultimosDigitos;
        Tipo = tipo;
        Apelido = Guarda.TextoOpcional(apelido, "Apelido", TamanhoMaximoApelido) ?? $"{bandeira} final {ultimosDigitos}";
        Ativo = true;
        DataCadastro = dataCadastro;
    }

    /// <summary>
    /// Cria o cartão a partir do número digitado, extraindo só bandeira e últimos 4 dígitos.
    /// O número completo é descartado ao final deste método.
    /// </summary>
    public static CartaoSimulado Criar(int clienteId, string numeroDigitado, TipoCartao tipo, string apelido, DateTime agora)
    {
        var digitos = new string((numeroDigitado ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digitos.Length is < 13 or > 19)
            throw new RegraDeNegocioException("Número do cartão deve ter entre 13 e 19 dígitos.");

        return new CartaoSimulado(clienteId, IdentificarBandeira(digitos), digitos[^QuantidadeUltimosDigitos..], tipo, apelido, agora);
    }

    public static string IdentificarBandeira(string digitos) => digitos[0] switch
    {
        '4' => "Visa",
        '5' => "Mastercard",
        '3' => "American Express",
        '6' => "Elo",
        _ => "Outra"
    };

    public bool AceitaForma(FormaPagamento forma) =>
        (forma == FormaPagamento.CartaoCredito && Tipo == TipoCartao.Credito) ||
        (forma == FormaPagamento.CartaoDebito && Tipo == TipoCartao.Debito);

    public void Inativar() => Ativo = false;

    public string ObterDescricao() => $"{Apelido} — {Bandeira} •••• {UltimosDigitos} ({Tipo.ParaTexto()})";
}
