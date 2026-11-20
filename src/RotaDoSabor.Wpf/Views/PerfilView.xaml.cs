namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

public partial class PerfilView : UserControl, ITelaComParametro
{
    private readonly PerfilController _controller;
    private readonly AutenticacaoController _autenticacao;

    public PerfilView(PerfilController controller, AutenticacaoController autenticacao)
    {
        InitializeComponent();
        _controller = controller;
        _autenticacao = autenticacao;
    }

    public async void AoNavegar(object? parametro)
    {
        var perfil = await _controller.CarregarAsync();
        if (perfil is null)
            return;

        TxtNome.Text = perfil.Nome;
        TxtTelefone.Text = perfil.Telefone;
        TxtEmail.Text = perfil.Email;
        TxtUsuario.Text = perfil.NomeUsuario;
        TxtDesde.Text = $"Cliente desde {perfil.DataCadastro:dd/MM/yyyy}";
    }

    private async void BtnSalvar_Click(object sender, RoutedEventArgs e) =>
        await _controller.SalvarAsync(TxtNome.Text, TxtTelefone.Text);

    private void BtnEnderecos_Click(object sender, RoutedEventArgs e) => _controller.AbrirEnderecos();

    private void BtnCartoes_Click(object sender, RoutedEventArgs e) => _controller.AbrirCartoes();

    private void BtnHistorico_Click(object sender, RoutedEventArgs e) => _controller.AbrirHistorico();

    private void BtnSair_Click(object sender, RoutedEventArgs e) => _autenticacao.Sair();

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
