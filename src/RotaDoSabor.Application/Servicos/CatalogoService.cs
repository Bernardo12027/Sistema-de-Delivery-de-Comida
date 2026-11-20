namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;

/// <summary>Consulta de restaurantes, categorias e cardápio (somente leitura).</summary>
public interface ICatalogoService
{
    Task<IReadOnlyList<CategoriaDto>> ListarCategoriasAsync();
    Task<IReadOnlyList<RestauranteDto>> ListarRestaurantesAsync(string? termoBusca, int? categoriaId);
    Task<CardapioDto> ObterCardapioAsync(int restauranteId);
}

public sealed class CatalogoService : ICatalogoService
{
    private readonly ICategoriaRepository _categorias;
    private readonly IRestauranteRepository _restaurantes;
    private readonly IProdutoRepository _produtos;

    public CatalogoService(ICategoriaRepository categorias, IRestauranteRepository restaurantes, IProdutoRepository produtos)
    {
        _categorias = categorias;
        _restaurantes = restaurantes;
        _produtos = produtos;
    }

    public async Task<IReadOnlyList<CategoriaDto>> ListarCategoriasAsync()
    {
        var categorias = await _categorias.ListarComRestaurantesAsync();
        return categorias.Select(categoria => new CategoriaDto(categoria.Id, categoria.Nome)).ToList();
    }

    public async Task<IReadOnlyList<RestauranteDto>> ListarRestaurantesAsync(string? termoBusca, int? categoriaId)
    {
        var termo = string.IsNullOrWhiteSpace(termoBusca) ? null : termoBusca.Trim();
        var restaurantes = await _restaurantes.ListarAsync(termo, categoriaId);
        return restaurantes.Select(restaurante => restaurante.ParaDto()).ToList();
    }

    public async Task<CardapioDto> ObterCardapioAsync(int restauranteId)
    {
        var restaurante = await _restaurantes.ObterPorIdAsync(restauranteId)
            ?? throw new NaoEncontradoException("Restaurante");

        var produtos = await _produtos.ListarPorRestauranteAsync(restauranteId);
        var produtosDto = produtos
            .OrderBy(produto => produto.ObterTipo() == "Bebida") // pratos primeiro, depois bebidas
            .ThenBy(produto => produto.Nome)
            .Select(produto => produto.ParaDto())
            .ToList();

        return new CardapioDto(restaurante.ParaDto(), produtosDto);
    }
}
