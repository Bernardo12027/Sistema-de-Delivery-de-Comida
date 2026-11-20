namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Domain.Excecoes;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>
/// Base dos controllers: centraliza o tratamento de erros (evita try/catch repetido em cada ação).
/// Regras de negócio violadas viram aviso; falhas técnicas (ex.: banco fora do ar) viram mensagem de erro.
/// </summary>
public abstract class ControllerBase
{
    protected ControllerBase(IDialogoService dialogo, INavegador navegador)
    {
        Dialogo = dialogo;
        Navegador = navegador;
    }

    protected IDialogoService Dialogo { get; }
    protected INavegador Navegador { get; }

    public void Voltar() => Navegador.Voltar();

    public void IrParaHome() => Navegador.Navegar(Tela.Home);

    protected async Task<bool> ExecutarAsync(Func<Task> acao)
    {
        try
        {
            await acao();
            return true;
        }
        catch (Exception ex)
        {
            TratarErro(ex);
            return false;
        }
    }

    protected async Task<T?> ExecutarAsync<T>(Func<Task<T>> acao) where T : class
    {
        try
        {
            return await acao();
        }
        catch (Exception ex)
        {
            TratarErro(ex);
            return null;
        }
    }

    protected bool Executar(Action acao)
    {
        try
        {
            acao();
            return true;
        }
        catch (Exception ex)
        {
            TratarErro(ex);
            return false;
        }
    }

    private void TratarErro(Exception ex)
    {
        if (ex is RegraDeNegocioException)
            Dialogo.Avisar(ex.Message);
        else
            Dialogo.Erro(MensagensErro.Traduzir(ex));
    }
}
