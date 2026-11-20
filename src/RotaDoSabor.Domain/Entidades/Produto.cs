namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Produto do cardápio. É abstrato: todo produto concreto é um <see cref="Prato"/> ou uma <see cref="Bebida"/>
/// (herança TPH no banco). Qualquer código que trabalha com Produto funciona com as duas subclasses (LSP).
/// </summary>
public abstract class Produto
{
    public const int TamanhoMaximoNome = 100;
    public const int TamanhoMaximoDescricao = 300;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public decimal Preco { get; private set; }
    public bool Disponivel { get; private set; }

    public int CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }

    public int RestauranteId { get; private set; }
    public Restaurante? Restaurante { get; private set; }

    public ICollection<ItemPedido> ItensPedido { get; private set; } = new List<ItemPedido>();

    protected Produto()
    {
    }

    protected Produto(string nome, string descricao, decimal preco, int categoriaId, int restauranteId, bool disponivel)
    {
        Nome = Guarda.TextoObrigatorio(nome, "Nome do produto", TamanhoMaximoNome);
        Descricao = Guarda.TextoObrigatorio(descricao, "Descrição do produto", TamanhoMaximoDescricao);
        Preco = Guarda.ValorPositivo(preco, "Preço");
        CategoriaId = Guarda.IdValido(categoriaId, "Categoria");
        RestauranteId = Guarda.IdValido(restauranteId, "Restaurante");
        Disponivel = disponivel;
    }

    /// <summary>"Prato" ou "Bebida" — usado na exibição do cardápio.</summary>
    public abstract string ObterTipo();

    /// <summary>Detalhes específicos de cada subtipo (tamanho/tempo ou volume/tipo).</summary>
    public abstract string ObterDetalhes();

    public void AlterarDisponibilidade(bool disponivel) => Disponivel = disponivel;
}
