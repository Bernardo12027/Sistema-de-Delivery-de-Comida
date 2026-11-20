namespace RotaDoSabor.Application.Dtos;

using RotaDoSabor.Application.Comum;

public sealed record CategoriaDto(int Id, string Nome);

public sealed record RestauranteDto(
    int Id,
    string Nome,
    string Categoria,
    decimal AvaliacaoMedia,
    int QuantidadeAvaliacoes,
    string Endereco,
    string Telefone,
    int TempoEntregaMinutos)
{
    public string AvaliacaoTexto => Formatacao.Estrelas(AvaliacaoMedia, QuantidadeAvaliacoes);
    public string TempoEntregaTexto => $"Entrega ~{TempoEntregaMinutos} min · Frete grátis";
}

public sealed record ProdutoDto(
    int Id,
    string Nome,
    string Descricao,
    decimal Preco,
    string Categoria,
    string Tipo,
    string Detalhes,
    bool Disponivel)
{
    public string PrecoTexto => Formatacao.Moeda(Preco);
    public string DisponibilidadeTexto => Disponivel ? "Disponível" : "Indisponível";
}

public sealed record CardapioDto(RestauranteDto Restaurante, IReadOnlyList<ProdutoDto> Produtos);
