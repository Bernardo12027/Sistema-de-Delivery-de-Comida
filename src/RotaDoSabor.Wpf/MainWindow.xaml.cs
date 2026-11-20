namespace RotaDoSabor.Wpf;

using System.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void ExibirConteudo(object conteudo) => AreaConteudo.Content = conteudo;
}
