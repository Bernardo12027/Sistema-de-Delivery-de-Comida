using System.Windows.Controls;

namespace RotaDoSabor.Navegacao;

/// <summary>Troca a tela exibida na janela principal.</summary>
public interface INavegador
{
    void DefinirContainer(ContentControl container);

    void NavegarPara<TView>() where TView : UserControl;
}
