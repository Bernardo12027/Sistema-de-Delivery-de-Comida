namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Wpf.Controllers;

public partial class CadastroView : UserControl
{
    private readonly AutenticacaoController _controller;

    public CadastroView(AutenticacaoController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        var dados = new CadastroClienteDto(TxtNome.Text, TxtEmail.Text, TxtUsuario.Text, TxtTelefone.Text,
            PwdSenha.Password, PwdConfirmacao.Password);

        BtnConfirmar.IsEnabled = false;
        try
        {
            await _controller.CadastrarAsync(dados);
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnJaTenhoConta_Click(object sender, RoutedEventArgs e) => _controller.IrParaLogin();

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
