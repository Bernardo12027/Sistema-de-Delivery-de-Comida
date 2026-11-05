using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Controllers;

namespace RotaDoSabor.Views;

/// <summary>"V" do MVC: só exibe a tela e repassa os cliques ao controller.</summary>
public partial class WelcomeView : UserControl
{
    private readonly AuthController _controller;

    public WelcomeView(AuthController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e) => _controller.MostrarLogin();

    private void BtnCadastro_Click(object sender, RoutedEventArgs e) => _controller.MostrarCadastro();
}
