namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class CarrinhoView : UserControl, ITelaComParametro
{
    private readonly CarrinhoController _controller;

    public CarrinhoView(CarrinhoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public void AoNavegar(object? parametro) => Atualizar();

    private void Atualizar()
    {
        var carrinho = _controller.Obter();
        DgItens.ItemsSource = carrinho.Itens;
        TxtRestaurante.Text = carrinho.EstaVazio ? string.Empty : $"Restaurante: {carrinho.NomeRestaurante}";
        TxtVazio.Visibility = carrinho.EstaVazio ? Visibility.Visible : Visibility.Collapsed;
        TxtTotal.Text = $"Total: {carrinho.TotalTexto}";
        BtnFinalizar.IsEnabled = !carrinho.EstaVazio;
    }

    private void BtnDiminuir_Click(object sender, RoutedEventArgs e) => ExecutarNoItem(sender, _controller.Diminuir);

    private void BtnAumentar_Click(object sender, RoutedEventArgs e) => ExecutarNoItem(sender, _controller.Aumentar);

    private void BtnRemover_Click(object sender, RoutedEventArgs e) => ExecutarNoItem(sender, _controller.Remover);

    private void ExecutarNoItem(object sender, Func<int, bool> acao)
    {
        if (sender is FrameworkElement { Tag: int produtoId } && acao(produtoId))
            Atualizar();
    }

    private void BtnLimpar_Click(object sender, RoutedEventArgs e)
    {
        if (_controller.Limpar())
            Atualizar();
    }

    private void BtnContinuar_Click(object sender, RoutedEventArgs e) => _controller.ContinuarComprando();

    private void BtnFinalizar_Click(object sender, RoutedEventArgs e) => _controller.IrParaFinalizacao();
}
