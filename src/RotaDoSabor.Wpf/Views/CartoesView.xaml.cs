namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class CartoesView : UserControl, ITelaComParametro
{
    private readonly CartaoController _controller;

    public CartoesView(CartaoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro) => await CarregarAsync();

    private async Task CarregarAsync()
    {
        var cartoes = await _controller.ListarAsync();
        LstCartoes.ItemsSource = cartoes;
        TxtVazio.Visibility = cartoes.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void BtnAdicionar_Click(object sender, RoutedEventArgs e)
    {
        var tipo = RbDebito.IsChecked == true ? TipoCartao.Debito : TipoCartao.Credito;
        var dados = new NovoCartaoDto(tipo, TxtNumero.Text, TxtNome.Text, TxtValidade.Text, PwdCvv.Password, TxtApelido.Text);

        if (!await _controller.AdicionarAsync(dados))
            return;

        // Os dados sensíveis digitados são apagados da tela logo após o cadastro.
        TxtNumero.Text = TxtNome.Text = TxtValidade.Text = TxtApelido.Text = string.Empty;
        PwdCvv.Clear();
        await CarregarAsync();
    }

    private async void BtnRemover_Click(object sender, RoutedEventArgs e)
    {
        if (LstCartoes.SelectedItem is CartaoDto cartao && await _controller.RemoverAsync(cartao))
            await CarregarAsync();
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
