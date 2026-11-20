namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class EnderecosView : UserControl, ITelaComParametro
{
    private readonly EnderecoController _controller;
    private int? _idEmEdicao;

    public EnderecosView(EnderecoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    public async void AoNavegar(object? parametro) => await CarregarAsync();

    private async Task CarregarAsync()
    {
        var enderecos = await _controller.ListarAsync();
        LstEnderecos.ItemsSource = enderecos;
        TxtVazio.Visibility = enderecos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        LimparFormulario();
    }

    private void LstEnderecos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LstEnderecos.SelectedItem is not EnderecoDto endereco)
            return;

        _idEmEdicao = endereco.Id;
        TxtModo.Text = "Editar endereço";
        TxtCep.Text = endereco.Cep;
        TxtLogradouro.Text = endereco.Logradouro;
        TxtNumero.Text = endereco.Numero;
        TxtComplemento.Text = endereco.Complemento ?? string.Empty;
        TxtBairro.Text = endereco.Bairro;
        TxtCidade.Text = endereco.Cidade;
        ChkPadrao.IsChecked = endereco.Padrao;
    }

    private void LimparFormulario()
    {
        _idEmEdicao = null;
        LstEnderecos.SelectedItem = null;
        TxtModo.Text = "Cadastrar endereço";
        TxtCep.Text = TxtLogradouro.Text = TxtNumero.Text = TxtComplemento.Text = TxtBairro.Text = TxtCidade.Text = string.Empty;
        ChkPadrao.IsChecked = false;
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e)
    {
        var dados = new SalvarEnderecoDto(_idEmEdicao, TxtCep.Text, TxtLogradouro.Text, TxtNumero.Text, TxtComplemento.Text,
            TxtBairro.Text, TxtCidade.Text, ChkPadrao.IsChecked == true);

        if (await _controller.SalvarAsync(dados))
            await CarregarAsync();
    }

    private void BtnNovo_Click(object sender, RoutedEventArgs e) => LimparFormulario();

    private async void BtnPadrao_Click(object sender, RoutedEventArgs e)
    {
        if (LstEnderecos.SelectedItem is EnderecoDto endereco && await _controller.DefinirPadraoAsync(endereco.Id))
            await CarregarAsync();
    }

    private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
    {
        if (LstEnderecos.SelectedItem is EnderecoDto endereco && await _controller.ExcluirAsync(endereco))
            await CarregarAsync();
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
