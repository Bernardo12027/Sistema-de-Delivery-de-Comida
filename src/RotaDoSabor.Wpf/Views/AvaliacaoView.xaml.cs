namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class AvaliacaoView : UserControl, ITelaComParametro
{
    private readonly AvaliacaoController _controller;
    private int _pedidoId;

    public AvaliacaoView(AvaliacaoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro)
    {
        if (parametro is not int pedidoId)
            return;

        _pedidoId = pedidoId;
        var info = await _controller.CarregarAsync(pedidoId);
        if (info is null)
            return;

        TxtTitulo.Text = $"Avaliar pedido #{info.PedidoId}";
        TxtRestaurante.Text = info.NomeRestaurante;
        TxtBloqueio.Text = info.Mensagem;
        TxtBloqueio.Visibility = info.PodeAvaliar ? Visibility.Collapsed : Visibility.Visible;
        PainelEstrelas.IsEnabled = info.PodeAvaliar;
        TxtComentario.IsEnabled = info.PodeAvaliar;
        BtnConfirmar.IsEnabled = info.PodeAvaliar;
    }

    private int? NotaSelecionada() =>
        PainelEstrelas.Children.OfType<RadioButton>()
            .Where(opcao => opcao.IsChecked == true)
            .Select(opcao => (int?)Convert.ToInt32(opcao.Tag))
            .FirstOrDefault();

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        BtnConfirmar.IsEnabled = false;
        try
        {
            await _controller.EnviarAsync(_pedidoId, NotaSelecionada(), TxtComentario.Text);
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
