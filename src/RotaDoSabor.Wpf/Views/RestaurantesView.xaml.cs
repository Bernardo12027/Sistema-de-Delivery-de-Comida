namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class RestaurantesView : UserControl, ITelaComParametro
{
    private readonly RestaurantesController _controller;
    private bool _carregando;

    public RestaurantesView(RestaurantesController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro)
    {
        var filtro = parametro as FiltroRestaurantes ?? new FiltroRestaurantes(null, null);

        _carregando = true;
        var categorias = await _controller.CarregarCategoriasAsync();
        CmbCategoria.ItemsSource = categorias;
        CmbCategoria.SelectedItem = categorias.FirstOrDefault(categoria => categoria.Id == (filtro.CategoriaId ?? 0));
        TxtBusca.Text = filtro.TermoBusca ?? string.Empty;
        _carregando = false;

        await AtualizarListaAsync();
    }

    private async Task AtualizarListaAsync()
    {
        var categoriaId = (CmbCategoria.SelectedItem as CategoriaDto)?.Id;
        var restaurantes = await _controller.FiltrarAsync(TxtBusca.Text, categoriaId);
        LstRestaurantes.ItemsSource = restaurantes;
        TxtVazio.Visibility = restaurantes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void BtnFiltrar_Click(object sender, RoutedEventArgs e) => await AtualizarListaAsync();

    private async void CmbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_carregando)
            await AtualizarListaAsync();
    }

    private void BtnVerCardapio_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int restauranteId })
            _controller.AbrirCardapio(restauranteId);
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();

    private void BtnCarrinho_Click(object sender, RoutedEventArgs e) => _controller.AbrirCarrinho();
}
