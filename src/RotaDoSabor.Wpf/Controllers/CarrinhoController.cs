namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Wpf.Navegacao;

public sealed class CarrinhoController : ControllerBase
{
    private readonly ICarrinhoService _carrinho;

    public CarrinhoController(ICarrinhoService carrinho, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _carrinho = carrinho;
    }

    public CarrinhoDto Obter() => _carrinho.Obter();

    public bool Aumentar(int produtoId) => Executar(() => _carrinho.Aumentar(produtoId));

    public bool Diminuir(int produtoId) => Executar(() => _carrinho.Diminuir(produtoId));

    public bool Remover(int produtoId) => Executar(() => _carrinho.Remover(produtoId));

    public bool Limpar()
    {
        if (_carrinho.Obter().EstaVazio || !Dialogo.Confirmar("Deseja remover todos os itens do carrinho?"))
            return false;
        return Executar(_carrinho.Limpar);
    }

    public void ContinuarComprando()
    {
        var carrinho = _carrinho.Obter();
        if (carrinho.RestauranteId is int restauranteId)
            Navegador.Navegar(Tela.Cardapio, restauranteId);
        else
            Navegador.Navegar(Tela.Home);
    }

    public void IrParaFinalizacao()
    {
        if (_carrinho.Obter().EstaVazio)
        {
            Dialogo.Avisar("Adicione ao menos um item ao carrinho para finalizar o pedido.");
            return;
        }
        Navegador.Navegar(Tela.Finalizacao);
    }
}
