namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class CardapioView : UserControl, ITelaComParametro
{
    private readonly CardapioController _controller;

    public CardapioView(CardapioController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro)
    {
        if (parametro is not int restauranteId)
            return;

        var cardapio = await _controller.CarregarAsync(restauranteId);
        if (cardapio is null)
            return;

        var restaurante = cardapio.Restaurante;
        TxtNomeRestaurante.Text = restaurante.Nome;
        TxtAvaliacao.Text = $"Avaliação: {restaurante.AvaliacaoTexto}";
        TxtInfo.Text = $"{restaurante.Categoria} · {restaurante.Endereco} · {restaurante.Telefone} · {restaurante.TempoEntregaTexto}";
        DgProdutos.ItemsSource = cardapio.Produtos;
        AtualizarResumoCarrinho();
    }

    private async void BtnAdicionar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: int produtoId } && await _controller.AdicionarAsync(produtoId))
            AtualizarResumoCarrinho();
    }

    private void AtualizarResumoCarrinho()
    {
        var carrinho = _controller.ObterCarrinho();
        TxtResumoCarrinho.Text = carrinho.EstaVazio
            ? "Carrinho vazio"
            : $"Carrinho ({carrinho.NomeRestaurante}): {carrinho.QuantidadeTotal} item(ns) — {carrinho.TotalTexto}";
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();

    private void BtnCarrinho_Click(object sender, RoutedEventArgs e) => _controller.AbrirCarrinho();
}
