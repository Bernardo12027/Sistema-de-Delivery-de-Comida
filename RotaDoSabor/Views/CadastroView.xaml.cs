using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Controllers;
using RotaDoSabor.Models;

namespace RotaDoSabor.Views;

public partial class CadastroView : UserControl
{
    private readonly AuthController _controller;

    public CadastroView(AuthController controller)
    {
        InitializeComponent();
        _controller = controller;

        Loaded += (_, _) => TxtUsuario.Focus();
    }

    private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
    {
        OcultarErro();
        BtnConfirmar.IsEnabled = false;

        try
        {
            var dados = new DadosCadastro(TxtUsuario.Text, TxtEmail.Text, TxtSenha.Password, TxtConfirmacao.Password);
            var resultado = await _controller.CadastrarAsync(dados);

            if (!resultado.Sucesso)
            {
                MostrarErro(resultado.Mensagem);
                return;
            }

            MessageBox.Show("Conta criada com sucesso! Agora é só entrar.", "Rota do Sabor",
                MessageBoxButton.OK, MessageBoxImage.Information);
            _controller.MostrarLogin();
        }
        finally
        {
            BtnConfirmar.IsEnabled = true;
        }
    }

    private void BtnVoltar_Click(object sender, RoutedEventArgs e) => _controller.MostrarInicial();

    private void MostrarErro(string mensagem)
    {
        TxtErro.Text = mensagem;
        CaixaErro.Visibility = Visibility.Visible;
    }

    private void OcultarErro() => CaixaErro.Visibility = Visibility.Collapsed;
}
