namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Login, cadastro e logout.</summary>
public sealed class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacao;

    public AutenticacaoController(IAutenticacaoService autenticacao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _autenticacao = autenticacao;
    }

    public async Task EntrarAsync(string emailOuUsuario, string senha)
    {
        if (await ExecutarAsync(() => _autenticacao.EntrarAsync(emailOuUsuario, senha)))
            Navegador.Navegar(Tela.Home);
    }

    public async Task<bool> CadastrarAsync(CadastroClienteDto dados)
    {
        var sucesso = await ExecutarAsync(() => _autenticacao.CadastrarAsync(dados));
        if (sucesso)
        {
            Dialogo.Informar("Cadastro realizado com sucesso! Faça login com seu usuário ou e-mail.");
            Navegador.Navegar(Tela.Login);
        }
        return sucesso;
    }

    public void Sair()
    {
        if (!Dialogo.Confirmar("Deseja sair da sua conta? O carrinho atual será esvaziado."))
            return;

        _autenticacao.Sair();
        Navegador.Navegar(Tela.Inicio);
    }

    public void IrParaLogin() => Navegador.Navegar(Tela.Login);

    public void IrParaCadastro() => Navegador.Navegar(Tela.Cadastro);
}
