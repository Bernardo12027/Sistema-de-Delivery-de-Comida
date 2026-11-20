namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Item de um pedido (classe associativa Pedido N-N Produto). Guarda preço e nome do momento da compra.</summary>
public class ItemPedido
{
    public const int QuantidadeMaxima = Carrinho.QuantidadeMaximaPorItem;

    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public Pedido? Pedido { get; private set; }
    public int ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    protected ItemPedido()
    {
    }

    public ItemPedido(int produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (quantidade is < 1 or > QuantidadeMaxima)
            throw new RegraDeNegocioException($"Quantidade deve estar entre 1 e {QuantidadeMaxima}.");

        ProdutoId = Guarda.IdValido(produtoId, "Produto");
        NomeProduto = Guarda.TextoObrigatorio(nomeProduto, "Nome do produto", Produto.TamanhoMaximoNome);
        Quantidade = quantidade;
        PrecoUnitario = Guarda.ValorPositivo(precoUnitario, "Preço unitário");
        Subtotal = PrecoUnitario * Quantidade;
    }
}
