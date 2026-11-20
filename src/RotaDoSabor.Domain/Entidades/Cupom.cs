namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Cupom de desconto percentual.</summary>
public class Cupom
{
    public const int TamanhoMaximoCodigo = 30;
    public const int TamanhoMaximoDescricao = 150;

    public int Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal PercentualDesconto { get; private set; }
    public DateTime DataValidade { get; private set; }
    public bool Ativo { get; private set; }

    public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();

    protected Cupom()
    {
    }

    public Cupom(string codigo, string descricao, decimal percentualDesconto, DateTime dataValidade, bool ativo = true)
    {
        if (percentualDesconto is <= 0 or > 100)
            throw new RegraDeNegocioException("Percentual de desconto deve estar entre 0 e 100.");

        Codigo = NormalizarCodigo(codigo);
        Descricao = Guarda.TextoObrigatorio(descricao, "Descrição do cupom", TamanhoMaximoDescricao);
        PercentualDesconto = percentualDesconto;
        DataValidade = dataValidade.Date;
        Ativo = ativo;
    }

    /// <summary>Válido quando ativo e a data atual não passou da data de validade (inclusive).</summary>
    public bool EstaValido(DateTime agora) => Ativo && agora.Date <= DataValidade;

    /// <summary>Desconto percentual arredondado a 2 casas e limitado ao total (nunca negativo).</summary>
    public decimal CalcularDesconto(decimal total)
    {
        if (total <= 0)
            return 0m;

        var desconto = Math.Round(total * PercentualDesconto / 100m, 2, MidpointRounding.AwayFromZero);
        return Math.Min(desconto, total);
    }

    public static string NormalizarCodigo(string codigo) =>
        Guarda.TextoObrigatorio(codigo, "Código do cupom", TamanhoMaximoCodigo).ToUpperInvariant();
}
