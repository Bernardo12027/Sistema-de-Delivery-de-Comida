namespace RotaDoSabor.Wpf.Navegacao;

using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Views;

/// <summary>
/// Troca o conteúdo da janela principal pela View correspondente à tela (criada via DI).
/// Mantém uma pilha simples para o botão "Voltar" e bloqueia telas internas sem login.
/// </summary>
public sealed class NavegadorWpf : INavegador
{
    private static readonly Dictionary<Tela, Type> TiposDeView = new()
    {
        [Tela.Inicio] = typeof(InicioView),
        [Tela.Login] = typeof(LoginView),
        [Tela.Cadastro] = typeof(CadastroView),
        [Tela.Home] = typeof(HomeView),
        [Tela.Restaurantes] = typeof(RestaurantesView),
        [Tela.Cardapio] = typeof(CardapioView),
        [Tela.Carrinho] = typeof(CarrinhoView),
        [Tela.Finalizacao] = typeof(FinalizacaoView),
        [Tela.Acompanhamento] = typeof(AcompanhamentoView),
        [Tela.Historico] = typeof(HistoricoView),
        [Tela.Avaliacao] = typeof(AvaliacaoView),
        [Tela.Enderecos] = typeof(EnderecosView),
        [Tela.Cartoes] = typeof(CartoesView),
        [Tela.Perfil] = typeof(PerfilView)
    };

    private static readonly HashSet<Tela> TelasPublicas = [Tela.Inicio, Tela.Login, Tela.Cadastro];

    private readonly IServiceProvider _servicos;
    private readonly MainWindow _janela;
    private readonly ISessaoUsuario _sessao;
    private readonly Stack<(Tela Tela, object? Parametro)> _historico = new();
    private (Tela Tela, object? Parametro)? _atual;

    public NavegadorWpf(IServiceProvider servicos, MainWindow janela, ISessaoUsuario sessao)
    {
        _servicos = servicos;
        _janela = janela;
        _sessao = sessao;
    }

    public void Navegar(Tela tela, object? parametro = null)
    {
        if (!TelasPublicas.Contains(tela) && !_sessao.EstaAutenticado)
            tela = Tela.Login;

        if (tela is Tela.Inicio or Tela.Home)
            _historico.Clear();
        else if (_atual is not null && _atual.Value.Tela != tela)
            _historico.Push(_atual.Value);

        Exibir(tela, parametro);
    }

    public void Voltar()
    {
        if (_historico.Count > 0)
        {
            var anterior = _historico.Pop();
            Exibir(anterior.Tela, anterior.Parametro);
            return;
        }

        Exibir(_sessao.EstaAutenticado ? Tela.Home : Tela.Inicio, null);
    }

    private void Exibir(Tela tela, object? parametro)
    {
        var view = (UserControl)_servicos.GetRequiredService(TiposDeView[tela]);
        _atual = (tela, parametro);
        _janela.ExibirConteudo(view);

        if (view is ITelaComParametro telaComParametro)
            telaComParametro.AoNavegar(parametro);
    }
}
