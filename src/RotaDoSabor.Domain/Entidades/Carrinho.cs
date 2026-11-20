namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Carrinho de compras da sessão. Regras: apenas um restaurante por vez, quantidades entre 1 e
/// <see cref="QuantidadeMaximaPorItem"/> e somente produtos disponíveis.
/// </summary>
public class Carrinho
{
    public const int QuantidadeMinimaPorItem = 1;
    public const int QuantidadeMaximaPorItem = 20;

    private readonly List<ItemCarrinho> _itens = new();

    public IReadOnlyList<ItemCarrinho> Itens => _itens;
    public int? RestauranteId { get; private set; }
    public string NomeRestaurante { get; private set; } = string.Empty;

    public bool EstaVazio => _itens.Count == 0;
    public decimal Total => _itens.Sum(item => item.Subtotal);
    public int QuantidadeTotal => _itens.Sum(item => item.Quantidade);

    public bool PertenceAOutroRestaurante(int restauranteId) => !EstaVazio && RestauranteId != restauranteId;

    public void Adicionar(Produto produto, string nomeRestaurante, int quantidade = 1)
    {
        if (!produto.Disponivel)
            throw new RegraDeNegocioException($"\"{produto.Nome}\" está indisponível no momento.");
        if (PertenceAOutroRestaurante(produto.RestauranteId))
            throw new RegraDeNegocioException("O carrinho já possui itens de outro restaurante.");

        ValidarQuantidade(quantidade);

        var existente = BuscarItem(produto.Id);
        if (existente is not null)
        {
            AlterarQuantidade(produto.Id, existente.Quantidade + quantidade);
            return;
        }

        RestauranteId = produto.RestauranteId;
        NomeRestaurante = nomeRestaurante;
        _itens.Add(new ItemCarrinho(produto.Id, produto.Nome, produto.Preco, quantidade));
    }

    public void Aumentar(int produtoId) => AlterarQuantidade(produtoId, ObterItem(produtoId).Quantidade + 1);

    public void Diminuir(int produtoId) => AlterarQuantidade(produtoId, ObterItem(produtoId).Quantidade - 1);

    public void AlterarQuantidade(int produtoId, int novaQuantidade)
    {
        ValidarQuantidade(novaQuantidade);
        ObterItem(produtoId).DefinirQuantidade(novaQuantidade);
    }

    public void Remover(int produtoId)
    {
        _itens.Remove(ObterItem(produtoId));
        if (EstaVazio)
            Limpar();
    }

    public void Limpar()
    {
        _itens.Clear();
        RestauranteId = null;
        NomeRestaurante = string.Empty;
    }

    private static void ValidarQuantidade(int quantidade)
    {
        if (quantidade < QuantidadeMinimaPorItem)
            throw new RegraDeNegocioException("A quantidade mínima é 1. Para tirar o item, use \"Remover\".");
        if (quantidade > QuantidadeMaximaPorItem)
            throw new RegraDeNegocioException($"A quantidade máxima por item é {QuantidadeMaximaPorItem}.");
    }

    private ItemCarrinho? BuscarItem(int produtoId) => _itens.FirstOrDefault(item => item.ProdutoId == produtoId);

    private ItemCarrinho ObterItem(int produtoId) =>
        BuscarItem(produtoId) ?? throw new RegraDeNegocioException("Item não encontrado no carrinho.");
}
