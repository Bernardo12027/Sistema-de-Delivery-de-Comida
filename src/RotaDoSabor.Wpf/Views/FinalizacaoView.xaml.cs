namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class FinalizacaoView : UserControl, ITelaComParametro
{
    private readonly FinalizacaoController _controller;
    private IReadOnlyList<CartaoDto> _cartoes = [];
    private decimal _subtotal;
    private CupomAplicadoDto? _cupom;

    public FinalizacaoView(FinalizacaoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro)
    {
        var dados = await _controller.CarregarAsync();
        if (dados is null)
            return;

        _subtotal = dados.Carrinho.Total;
        _cartoes = dados.Cartoes;
        TxtRestaurante.Text = $"Restaurante: {dados.Carrinho.NomeRestaurante}";
        LstItens.ItemsSource = dados.Carrinho.Itens;

        CmbEndereco.ItemsSource = dados.Enderecos;
        CmbEndereco.SelectedItem = dados.Enderecos.FirstOrDefault(endereco => endereco.Padrao) ?? dados.Enderecos.FirstOrDefault();

        var previsao = DateTime.Now.AddMinutes(dados.TempoEstimadoMinutos);
        TxtTempo.Text = $"Tempo estimado de preparo + entrega: {dados.TempoEstimadoMinutos} min (previsão {Formatacao.Hora(previsao)})";

        RbPix.IsChecked = true;
        AtualizarResumo();
    }

    private FormaPagamento? FormaSelecionada() =>
        RbPix.IsChecked == true ? FormaPagamento.Pix :
        RbCredito.IsChecked == true ? FormaPagamento.CartaoCredito :
        RbDebito.IsChecked == true ? FormaPagamento.CartaoDebito :
        RbDinheiro.IsChecked == true ? FormaPagamento.Dinheiro : null;

    private void FormaPagamento_Checked(object sender, RoutedEventArgs e)
    {
        var forma = FormaSelecionada();
        var cartoesCompativeis = FinalizacaoController.FiltrarCartoes(_cartoes, forma);
        var usaCartao = forma is FormaPagamento.CartaoCredito or FormaPagamento.CartaoDebito;

        CmbCartao.ItemsSource = cartoesCompativeis;
        CmbCartao.SelectedItem = cartoesCompativeis.FirstOrDefault();
        CmbCartao.IsEnabled = usaCartao;
        LblCartao.Content = usaCartao && cartoesCompativeis.Count == 0
            ? "Cartão (nenhum cartão deste tipo cadastrado)"
            : "Cartão";
    }

    private async void BtnAplicarCupom_Click(object sender, RoutedEventArgs e)
    {
        var cupom = await _controller.AplicarCupomAsync(TxtCupom.Text);
        if (cupom is null)
            return;

        _cupom = cupom;
        TxtCupomInfo.Text = $"{cupom.Resumo} — {cupom.Descricao}";
        AtualizarResumo();
    }

    private void BtnRemoverCupom_Click(object sender, RoutedEventArgs e)
    {
        _cupom = null;
        TxtCupom.Text = string.Empty;
        TxtCupomInfo.Text = "Nenhum cupom aplicado.";
        AtualizarResumo();
    }

    private void AtualizarResumo()
    {
        var desconto = _cupom?.Desconto ?? 0m;
        TxtSubtotal.Text = $"Subtotal: {Formatacao.Moeda(_subtotal)}";
        TxtDesconto.Text = $"Desconto: {Formatacao.Moeda(desconto)}";
        TxtTotal.Text = $"Total: {Formatacao.Moeda(_subtotal - desconto)}";
    }

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        BtnConfirmar.IsEnabled = false; // evita pedido duplicado por duplo clique
        try
        {
            var cartao = CmbCartao.IsEnabled ? CmbCartao.SelectedItem as CartaoDto : null;
            await _controller.ConfirmarAsync(CmbEndereco.SelectedItem as EnderecoDto, FormaSelecionada(), cartao, _cupom?.Codigo);
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnEnderecos_Click(object sender, RoutedEventArgs e) => _controller.GerenciarEnderecos();

    private void BtnCartoes_Click(object sender, RoutedEventArgs e) => _controller.GerenciarCartoes();

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
