namespace RotaDoSabor.Domain.Entidades;

/// <summary>Item do carrinho em memória (o carrinho não é persistido; vira Pedido ao finalizar).</summary>
public class ItemCarrinho
{
    public int ProdutoId { get; }
    public string NomeProduto { get; }
    public decimal PrecoUnitario { get; }
    public int Quantidade { get; private set; }
    public decimal Subtotal => PrecoUnitario * Quantidade;

    internal ItemCarrinho(int produtoId, string nomeProduto, decimal precoUnitario, int quantidade)
    {
        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }

    internal void DefinirQuantidade(int quantidade) => Quantidade = quantidade;
}
