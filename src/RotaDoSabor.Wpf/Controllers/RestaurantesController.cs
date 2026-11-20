namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Lista de restaurantes com busca e filtro por categoria.</summary>
public sealed class RestaurantesController : ControllerBase
{
    /// <summary>Item "Todos" do filtro (Id 0 = sem filtro).</summary>
    public static readonly CategoriaDto CategoriaTodos = new(0, "Todos");

    private readonly ICatalogoService _catalogo;

    public RestaurantesController(ICatalogoService catalogo, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _catalogo = catalogo;
    }

    public async Task<IReadOnlyList<CategoriaDto>> CarregarCategoriasAsync()
    {
        var categorias = await ExecutarAsync(_catalogo.ListarCategoriasAsync) ?? [];
        return [CategoriaTodos, .. categorias];
    }

    public async Task<IReadOnlyList<RestauranteDto>> FiltrarAsync(string? termoBusca, int? categoriaId)
    {
        var categoria = categoriaId is null or 0 ? (int?)null : categoriaId;
        return await ExecutarAsync(() => _catalogo.ListarRestaurantesAsync(termoBusca, categoria)) ?? [];
    }

    public void AbrirCardapio(int restauranteId) => Navegador.Navegar(Tela.Cardapio, restauranteId);

    public void AbrirCarrinho() => Navegador.Navegar(Tela.Carrinho);
}
