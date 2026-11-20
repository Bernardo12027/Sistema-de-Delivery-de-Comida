namespace RotaDoSabor.Wpf.Navegacao;

/// <summary>
/// Navegação entre telas. Os controllers dependem desta interface (e não de classes WPF),
/// o que mantém a lógica de fluxo separada da interface gráfica.
/// </summary>
public interface INavegador
{
    void Navegar(Tela tela, object? parametro = null);
    void Voltar();
}
