namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Cardápio do restaurante e inclusão de itens no carrinho.</summary>
public sealed class CardapioController : ControllerBase
{
    private readonly ICatalogoService _catalogo;
    private readonly ICarrinhoService _carrinho;

    public CardapioController(ICatalogoService catalogo, ICarrinhoService carrinho, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _catalogo = catalogo;
        _carrinho = carrinho;
    }

    public Task<CardapioDto?> CarregarAsync(int restauranteId) => ExecutarAsync(() => _catalogo.ObterCardapioAsync(restauranteId));

    public CarrinhoDto ObterCarrinho() => _carrinho.Obter();

    /// <summary>Adiciona 1 unidade; se o carrinho for de outro restaurante, pede confirmação antes de substituir.</summary>
    public async Task<bool> AdicionarAsync(int produtoId)
    {
        return await ExecutarAsync(async () =>
        {
            var resultado = await _carrinho.AdicionarAsync(produtoId, substituirCarrinhoAtual: false);
            if (resultado != ResultadoAdicaoCarrinho.ConfirmacaoTrocaRestauranteNecessaria)
                return;

            var carrinhoAtual = _carrinho.Obter();
            var confirmou = Dialogo.Confirmar(
                $"Seu carrinho tem itens de \"{carrinhoAtual.NomeRestaurante}\". Um pedido só pode ter itens de um restaurante.\n\n" +
                "Deseja esvaziar o carrinho e adicionar este item?",
                "Trocar de restaurante?");

            if (confirmou)
                await _carrinho.AdicionarAsync(produtoId, substituirCarrinhoAtual: true);
        });
    }

    public void AbrirCarrinho() => Navegador.Navegar(Tela.Carrinho);
}
