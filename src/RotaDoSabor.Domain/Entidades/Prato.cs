namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;
using RotaDoSabor.Domain.Interfaces;

/// <summary>Produto que precisa de preparo (implementa <see cref="IPreparacao"/>).</summary>
public class Prato : Produto, IPreparacao
{
    public const int TamanhoMaximoTamanho = 30;

    public int TempoPreparo { get; private set; }
    public string Tamanho { get; private set; } = string.Empty;

    protected Prato()
    {
    }

    public Prato(string nome, string descricao, decimal preco, int categoriaId, int restauranteId, bool disponivel,
        int tempoPreparo, string tamanho)
        : base(nome, descricao, preco, categoriaId, restauranteId, disponivel)
    {
        TempoPreparo = tempoPreparo > 0 ? tempoPreparo : throw new RegraDeNegocioException("Tempo de preparo deve ser maior que zero.");
        Tamanho = Guarda.TextoObrigatorio(tamanho, "Tamanho", TamanhoMaximoTamanho);
    }

    public string Preparar() => $"Preparando {Nome} ({Tamanho}) — aproximadamente {TempoPreparo} min.";

    public override string ObterTipo() => "Prato";

    public override string ObterDetalhes() => $"{Tamanho} · preparo {TempoPreparo} min";
}
