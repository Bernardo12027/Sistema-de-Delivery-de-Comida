namespace RotaDoSabor.Application.Dtos;

using RotaDoSabor.Application.Comum;

public sealed record ItemCarrinhoDto(int ProdutoId, string NomeProduto, decimal PrecoUnitario, int Quantidade, decimal Subtotal)
{
    public string PrecoUnitarioTexto => Formatacao.Moeda(PrecoUnitario);
    public string SubtotalTexto => Formatacao.Moeda(Subtotal);
}

public sealed record CarrinhoDto(int? RestauranteId, string NomeRestaurante, IReadOnlyList<ItemCarrinhoDto> Itens, decimal Total, int QuantidadeTotal)
{
    public bool EstaVazio => Itens.Count == 0;
    public string TotalTexto => Formatacao.Moeda(Total);
}

public enum ResultadoAdicaoCarrinho
{
    Adicionado,
    ConfirmacaoTrocaRestauranteNecessaria
}
