namespace RotaDoSabor.Wpf.Navegacao;

using System.Windows;

public sealed class DialogoServiceWpf : IDialogoService
{
    private const string TituloApp = "Rota do Sabor";

    public bool Confirmar(string mensagem, string titulo = "Confirmação") =>
        MessageBox.Show(mensagem, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public void Informar(string mensagem) =>
        MessageBox.Show(mensagem, TituloApp, MessageBoxButton.OK, MessageBoxImage.Information);

    public void Avisar(string mensagem) =>
        MessageBox.Show(mensagem, TituloApp, MessageBoxButton.OK, MessageBoxImage.Warning);

    public void Erro(string mensagem) =>
        MessageBox.Show(mensagem, TituloApp, MessageBoxButton.OK, MessageBoxImage.Error);
}
