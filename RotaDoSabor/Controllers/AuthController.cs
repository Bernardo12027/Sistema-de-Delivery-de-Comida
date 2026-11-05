using System.Diagnostics;
using RotaDoSabor.Common;
using RotaDoSabor.Models;
using RotaDoSabor.Navegacao;
using RotaDoSabor.Services;
using RotaDoSabor.Views;

namespace RotaDoSabor.Controllers;

/// <summary>
/// "C" do MVC: recebe as ações da View (clique em Confirmar, Sair...), chama os serviços
/// (regras de negócio) e decide qual tela vem a seguir. Não contém regra de negócio nem código de tela.
/// </summary>
public class AuthController
{
    private const string ErroInesperado = "Ocorreu um erro inesperado. Tente novamente.";

    private readonly IAuthService _auth;
    private readonly ISessaoUsuario _sessao;
    private readonly INavegador _navegador;

    public AuthController(IAuthService auth, ISessaoUsuario sessao, INavegador navegador)
    {
        _auth = auth;
        _sessao = sessao;
        _navegador = navegador;
    }

    // ----- Navegação -----

    public void MostrarInicial() => _navegador.NavegarPara<WelcomeView>();

    public void MostrarLogin() => _navegador.NavegarPara<LoginView>();

    public void MostrarCadastro() => _navegador.NavegarPara<CadastroView>();

    public void MostrarHome() => _navegador.NavegarPara<HomeView>();

    // ----- Ações -----

    public async Task<Resultado> EntrarAsync(string identificador, string senha)
    {
        try
        {
            var resultado = await _auth.EntrarAsync(identificador, senha);
            if (resultado.Sucesso)
                _sessao.Iniciar(resultado.Valor!);

            return resultado;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return Resultado.Falha(ErroInesperado);
        }
    }

    public async Task<Resultado> CadastrarAsync(DadosCadastro dados)
    {
        try
        {
            return await _auth.CadastrarAsync(dados);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return Resultado.Falha(ErroInesperado);
        }
    }

    public void Sair()
    {
        _sessao.Encerrar();
        MostrarInicial();
    }
}
