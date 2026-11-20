namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Domain.Entidades;

/// <summary>Conversões Entidade → DTO usadas por vários serviços (evita duplicação).</summary>
internal static class Mapeamentos
{
    public static RestauranteDto ParaDto(this Restaurante restaurante) => new(
        restaurante.Id,
        restaurante.Nome,
        restaurante.Categoria?.Nome ?? string.Empty,
        restaurante.AvaliacaoMedia,
        restaurante.QuantidadeAvaliacoes,
        restaurante.Endereco,
        restaurante.Telefone,
        restaurante.TempoEntregaMinutos);

    public static ProdutoDto ParaDto(this Produto produto) => new(
        produto.Id,
        produto.Nome,
        produto.Descricao,
        produto.Preco,
        produto.Categoria?.Nome ?? string.Empty,
        produto.ObterTipo(),
        produto.ObterDetalhes(),
        produto.Disponivel);

    public static EnderecoDto ParaDto(this Endereco endereco) => new(
        endereco.Id,
        Endereco.FormatarCep(endereco.Cep),
        endereco.Logradouro,
        endereco.Numero,
        endereco.Complemento,
        endereco.Bairro,
        endereco.Cidade,
        endereco.Padrao,
        endereco.ObterDescricao());

    public static CartaoDto ParaDto(this CartaoSimulado cartao) => new(
        cartao.Id,
        cartao.Apelido,
        cartao.Bandeira,
        cartao.UltimosDigitos,
        cartao.Tipo,
        cartao.ObterDescricao());

    public static CarrinhoDto ParaDto(this Carrinho carrinho) => new(
        carrinho.RestauranteId,
        carrinho.NomeRestaurante,
        carrinho.Itens.Select(item => new ItemCarrinhoDto(item.ProdutoId, item.NomeProduto, item.PrecoUnitario, item.Quantidade, item.Subtotal)).ToList(),
        carrinho.Total,
        carrinho.QuantidadeTotal);

    public static ItemPedidoDto ParaDto(this ItemPedido item) => new(
        item.ProdutoId,
        item.NomeProduto,
        item.Quantidade,
        item.PrecoUnitario,
        item.Subtotal);
}
