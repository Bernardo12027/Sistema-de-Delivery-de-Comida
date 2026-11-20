namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Bebida: não precisa de preparo, por isso não implementa IPreparacao (ISP).</summary>
public class Bebida : Produto
{
    public const int TamanhoMaximoTipo = 30;

    /// <summary>Volume em mililitros.</summary>
    public int Volume { get; private set; }

    /// <summary>Tipo da bebida: Lata, Garrafa, Suco natural...</summary>
    public string Tipo { get; private set; } = string.Empty;

    protected Bebida()
    {
    }

    public Bebida(string nome, string descricao, decimal preco, int categoriaId, int restauranteId, bool disponivel,
        int volume, string tipo)
        : base(nome, descricao, preco, categoriaId, restauranteId, disponivel)
    {
        Volume = volume > 0 ? volume : throw new RegraDeNegocioException("Volume deve ser maior que zero.");
        Tipo = Guarda.TextoObrigatorio(tipo, "Tipo da bebida", TamanhoMaximoTipo);
    }

    public override string ObterTipo() => "Bebida";

    public override string ObterDetalhes() => $"{Volume} ml · {Tipo}";
}
