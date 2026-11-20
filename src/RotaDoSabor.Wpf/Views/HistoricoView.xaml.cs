namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class HistoricoView : UserControl, ITelaComParametro
{
    private readonly HistoricoController _controller;
    private readonly PedidoController _pedidos;

    public HistoricoView(HistoricoController controller, PedidoController pedidos)
    {
        InitializeComponent();
        _controller = controller;
        _pedidos = pedidos;
    }

    public async void AoNavegar(object? parametro) => await CarregarAsync();

    private async Task CarregarAsync()
    {
        var historico = await _controller.CarregarAsync();
        DgHistorico.ItemsSource = historico;
        TxtVazio.Visibility = historico.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void BtnDetalhes_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int pedidoId })
            _controller.VerDetalhes(pedidoId);
    }

    private void BtnAvaliar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int pedidoId })
            _controller.Avaliar(pedidoId);
    }

    private async void BtnRepetir_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int pedidoId })
            await _pedidos.RepetirAsync(pedidoId);
    }

    private async void BtnAtualizar_Click(object sender, RoutedEventArgs e) => await CarregarAsync();

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
