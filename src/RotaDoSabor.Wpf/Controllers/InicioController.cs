namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Wpf.Navegacao;

public sealed class InicioController : ControllerBase
{
    public InicioController(IDialogoService dialogo, INavegador navegador) : base(dialogo, navegador)
    {
    }

    public void IrParaLogin() => Navegador.Navegar(Tela.Login);

    public void IrParaCadastro() => Navegador.Navegar(Tela.Cadastro);
}
