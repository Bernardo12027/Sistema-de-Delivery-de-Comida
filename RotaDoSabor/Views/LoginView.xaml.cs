using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Controllers;

namespace RotaDoSabor.Views;

public partial class LoginView : UserControl
{
    private readonly AuthController _controller;

    public LoginView(AuthController controller)
    {
        InitializeComponent();
        _controller = controller;

        Loaded += (_, _) => TxtIdentificador.Focus();
    }

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        OcultarErro();
        BtnConfirmar.IsEnabled = false; // evita cliques duplos enquanto valida

        try
        {
            var resultado = await _controller.EntrarAsync(TxtIdentificador.Text, TxtSenha.Password);

            if (resultado.Sucesso)
                _controller.MostrarHome();
            else
                MostrarErro(resultado.Mensagem);
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.MostrarInicial();

    private void BtnIrCadastro_Click(object sender, RoutedEventArgs e) => _controller.MostrarCadastro();

    private void MostrarErro(string mensagem)
    {
        TxtErro.Text = mensagem;
        CaixaErro.Visibility = Visibility.Visible;
    }

    private void OcultarErro() => CaixaErro.Visibility = Visibility.Collapsed;
}
