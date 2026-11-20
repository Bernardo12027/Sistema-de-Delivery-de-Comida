namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Página inicial: pesquisa, categorias e menu lateral.</summary>
public sealed class HomeController : ControllerBase
{
    private readonly ICatalogoService _catalogo;
    private readonly ISessaoUsuario _sessao;

    public HomeController(ICatalogoService catalogo, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _catalogo = catalogo;
        _sessao = sessao;
    }

    public string Saudacao => $"Olá, {_sessao.NomeCliente}! O que vamos pedir hoje?";

    public string TextoBotaoCarrinho => $"Carrinho ({_sessao.Carrinho.QuantidadeTotal})";

    public Task<IReadOnlyList<CategoriaDto>?> CarregarCategoriasAsync() =>
        ExecutarAsync(_catalogo.ListarCategoriasAsync);

    public void Pesquisar(string termo) => Navegador.Navegar(Tela.Restaurantes, new FiltroRestaurantes(termo, null));

    /// <summary>categoriaId nulo = "Todos".</summary>
    public void SelecionarCategoria(int? categoriaId) => Navegador.Navegar(Tela.Restaurantes, new FiltroRestaurantes(null, categoriaId));

    public void AbrirCarrinho() => Navegador.Navegar(Tela.Carrinho);
    public void AbrirEnderecos() => Navegador.Navegar(Tela.Enderecos);
    public void AbrirCartoes() => Navegador.Navegar(Tela.Cartoes);
    public void AbrirHistorico() => Navegador.Navegar(Tela.Historico);
    public void AbrirPerfil() => Navegador.Navegar(Tela.Perfil);
}
