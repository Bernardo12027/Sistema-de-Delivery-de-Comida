namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class HomeView : UserControl, ITelaComParametro
{
    private readonly HomeController _controller;
    private readonly AutenticacaoController _autenticacao;

    public HomeView(HomeController controller, AutenticacaoController autenticacao)
    {
        InitializeComponent();
        _controller = controller;
        _autenticacao = autenticacao;
    }

    public async void AoNavegar(object? parametro)
    {
        TxtSaudacao.Text = _controller.Saudacao;
        BtnCarrinho.Content = _controller.TextoBotaoCarrinho;
        ListaCategorias.ItemsSource = await _controller.CarregarCategoriasAsync();
    }

    private void BtnMenu_Click(object sender, RoutedEventArgs e) =>
        PainelMenu.Visibility = PainelMenu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

    private void BtnPesquisar_Click(object sender, RoutedEventArgs e) => _controller.Pesquisar(TxtPesquisa.Text);

    private void BtnCategoria_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int categoriaId })
            _controller.SelecionarCategoria(categoriaId);
    }

    private void BtnTodos_Click(object sender, RoutedEventArgs e) => _controller.SelecionarCategoria(null);

    private void BtnCarrinho_Click(object sender, RoutedEventArgs e) => _controller.AbrirCarrinho();

    private void BtnEnderecos_Click(object sender, RoutedEventArgs e) => _controller.AbrirEnderecos();

    private void BtnCartoes_Click(object sender, RoutedEventArgs e) => _controller.AbrirCartoes();

    private void BtnHistorico_Click(object sender, RoutedEventArgs e) => _controller.AbrirHistorico();

    private void BtnPerfil_Click(object sender, RoutedEventArgs e) => _controller.AbrirPerfil();

    private void BtnSair_Click(object sender, RoutedEventArgs e) => _autenticacao.Sair();
}
