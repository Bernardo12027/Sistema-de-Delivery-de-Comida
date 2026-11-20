namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Endereço de entrega do cliente. Endereços usados em pedidos não são apagados fisicamente:
/// são inativados para preservar o histórico.
/// </summary>
public class Endereco
{
    public const int TamanhoMaximoLogradouro = 150;
    public const int TamanhoMaximoNumero = 10;
    public const int TamanhoMaximoComplemento = 100;
    public const int TamanhoMaximoBairro = 80;
    public const int TamanhoMaximoCidade = 80;
    public const int TamanhoCep = 8;

    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;

    /// <summary>CEP com 8 dígitos, sem hífen.</summary>
    public string Cep { get; private set; } = string.Empty;
    public bool Padrao { get; private set; }
    public bool Ativo { get; private set; }

    public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();

    protected Endereco()
    {
    }

    public Endereco(int clienteId, string cep, string logradouro, string numero, string? complemento, string bairro, string cidade)
    {
        ClienteId = Guarda.IdValido(clienteId, "Cliente");
        Ativo = true;
        Atualizar(cep, logradouro, numero, complemento, bairro, cidade);
    }

    public void Atualizar(string cep, string logradouro, string numero, string? complemento, string bairro, string cidade)
    {
        Cep = NormalizarCep(cep);
        Logradouro = Guarda.TextoObrigatorio(logradouro, "Logradouro", TamanhoMaximoLogradouro);
        Numero = Guarda.TextoObrigatorio(numero, "Número", TamanhoMaximoNumero);
        Complemento = Guarda.TextoOpcional(complemento, "Complemento", TamanhoMaximoComplemento);
        Bairro = Guarda.TextoObrigatorio(bairro, "Bairro", TamanhoMaximoBairro);
        Cidade = Guarda.TextoObrigatorio(cidade, "Cidade", TamanhoMaximoCidade);
    }

    /// <summary>NOVA FUNCIONALIDADE: definir endereço padrão.</summary>
    public void DefinirComoPadrao() => Padrao = true;

    public void RemoverPadrao() => Padrao = false;

    public void Inativar()
    {
        Ativo = false;
        Padrao = false;
    }

    public string ObterDescricao()
    {
        var complemento = string.IsNullOrWhiteSpace(Complemento) ? string.Empty : $" ({Complemento})";
        return $"{Logradouro}, {Numero}{complemento} - {Bairro}, {Cidade} - CEP {FormatarCep(Cep)}";
    }

    public static string NormalizarCep(string? cep)
    {
        var digitos = new string((cep ?? string.Empty).Where(char.IsDigit).ToArray());
        if (digitos.Length != TamanhoCep)
            throw new RegraDeNegocioException("CEP deve ter 8 dígitos.");
        return digitos;
    }

    public static string FormatarCep(string cep) =>
        cep.Length == TamanhoCep ? $"{cep[..5]}-{cep[5..]}" : cep;
}
