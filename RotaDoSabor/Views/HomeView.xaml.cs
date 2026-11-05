using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Controllers;
using RotaDoSabor.Services;

namespace RotaDoSabor.Views;

public partial class HomeView : UserControl
{
    private readonly AuthController _controller;

    public HomeView(AuthController controller, ISessaoUsuario sessao)
    {
        InitializeComponent();
        _controller = controller;

        TxtBoasVindas.Text = $"Olá, {sessao.UsuarioLogado?.NomeUsuario}!";
    }

    private void BtnSair_Click(object sender, RoutedEventArgs e) => _controller.Sair();
}
