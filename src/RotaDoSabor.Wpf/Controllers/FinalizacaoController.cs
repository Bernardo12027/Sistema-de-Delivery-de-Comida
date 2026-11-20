namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Dados necessários para montar a tela de finalização.</summary>
public sealed record DadosFinalizacao(
    CarrinhoDto Carrinho,
    IReadOnlyList<EnderecoDto> Enderecos,
    IReadOnlyList<CartaoDto> Cartoes,
    int TempoEstimadoMinutos);

/// <summary>Finalização do pedido: endereço, pagamento simulado e cupom.</summary>
public sealed class FinalizacaoController : ControllerBase
{
    private readonly ICheckoutService _checkout;
    private readonly IEnderecoService _enderecos;
    private readonly ICartaoService _cartoes;
    private readonly ICupomService _cupons;
    private readonly ICarrinhoService _carrinho;
    private readonly ISessaoUsuario _sessao;

    public FinalizacaoController(ICheckoutService checkout, IEnderecoService enderecos, ICartaoService cartoes,
        ICupomService cupons, ICarrinhoService carrinho, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _checkout = checkout;
        _enderecos = enderecos;
        _cartoes = cartoes;
        _cupons = cupons;
        _carrinho = carrinho;
        _sessao = sessao;
    }

    public Task<DadosFinalizacao?> CarregarAsync() => ExecutarAsync(async () =>
    {
        var clienteId = _sessao.ObterClienteIdObrigatorio();
        var enderecos = await _enderecos.ListarAsync(clienteId);
        var cartoes = await _cartoes.ListarAsync(clienteId);
        var tempo = await _checkout.CalcularTempoEstimadoAsync(_sessao.Carrinho);
        return new DadosFinalizacao(_carrinho.Obter(), enderecos, cartoes, tempo);
    });

    /// <summary>Cartões compatíveis com a forma escolhida (crédito ou débito).</summary>
    public static IReadOnlyList<CartaoDto> FiltrarCartoes(IEnumerable<CartaoDto> cartoes, FormaPagamento? forma)
    {
        var tipo = forma switch
        {
            FormaPagamento.CartaoCredito => TipoCartao.Credito,
            FormaPagamento.CartaoDebito => TipoCartao.Debito,
            _ => (TipoCartao?)null
        };
        return tipo is null ? [] : cartoes.Where(cartao => cartao.Tipo == tipo).ToList();
    }

    public Task<CupomAplicadoDto?> AplicarCupomAsync(string codigo) =>
        ExecutarAsync(() => _cupons.ValidarAsync(codigo, _sessao.Carrinho.Total));

    public async Task ConfirmarAsync(EnderecoDto? endereco, FormaPagamento? forma, CartaoDto? cartao, string? codigoCupom)
    {
        if (endereco is null)
        {
            Dialogo.Avisar("Selecione o endereço de entrega (ou cadastre um em \"Gerenciar endereços\").");
            return;
        }
        if (forma is null)
        {
            Dialogo.Avisar("Selecione a forma de pagamento.");
            return;
        }

        var resultado = await ExecutarAsync(() => _checkout.FinalizarAsync(
            _sessao.ObterClienteIdObrigatorio(),
            _sessao.Carrinho,
            new FinalizarPedidoDto(endereco.Id, forma.Value, cartao?.Id, codigoCupom)));

        if (resultado is null)
            return;

        if (resultado.PagamentoRecusado)
        {
            Dialogo.Avisar(resultado.Mensagem);
            return;
        }

        Dialogo.Informar(resultado.Mensagem);
        Navegador.Navegar(Tela.Acompanhamento, resultado.PedidoId);
    }

    public void GerenciarEnderecos() => Navegador.Navegar(Tela.Enderecos);

    public void GerenciarCartoes() => Navegador.Navegar(Tela.Cartoes);
}
