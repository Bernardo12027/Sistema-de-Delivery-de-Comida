namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;

public partial class LoginView : UserControl
{
    private readonly AutenticacaoController _controller;

    public LoginView(AutenticacaoController controller)
    {
        InitializeComponent();
        _controller = controller;
        Loaded += (_, _) => TxtLogin.Focus();
    }

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        BtnConfirmar.IsEnabled = false;
        try
        {
            await _controller.EntrarAsync(TxtLogin.Text, PwdSenha.Password);
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnCriarConta_Click(object sender, RoutedEventArgs e) => _controller.IrParaCadastro();

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.Voltar();
}
