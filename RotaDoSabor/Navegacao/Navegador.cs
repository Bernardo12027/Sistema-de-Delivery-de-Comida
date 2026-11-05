using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace RotaDoSabor.Navegacao;

/// <summary>
/// Navegação entre telas: pede a View ao contêiner de injeção de dependência e a coloca na janela.
/// SOLID (O): para criar uma nova tela basta registrá-la no App.xaml.cs; este código não muda.
/// </summary>
public class Navegador : INavegador
{
    private readonly IServiceProvider _servicos;
    private ContentControl? _container;

    public Navegador(IServiceProvider servicos)
    {
        _servicos = servicos;
    }

    public void DefinirContainer(ContentControl container) => _container = container;

    public void NavegarPara<TView>() where TView : UserControl
    {
        if (_container is null)
            throw new InvalidOperationException("O contêiner de navegação ainda não foi definido.");

        _container.Content = _servicos.GetRequiredService<TView>();
    }
}
