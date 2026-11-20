namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using RotaDoSabor.Wpf.Controllers;

public partial class InicioView : UserControl
{
    private readonly InicioController _controller;

    public InicioView(InicioController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e) => _controller.IrParaLogin();

    private void BtnCadastro_Click(object sender, RoutedEventArgs e) => _controller.IrParaCadastro();
}
