using System.Windows;
using RotaDoSabor.Navegacao;

namespace RotaDoSabor;

public partial class MainWindow : Window
{
    public MainWindow(INavegador navegador)
    {
        InitializeComponent();
        navegador.DefinirContainer(ConteudoPrincipal);
    }
}
