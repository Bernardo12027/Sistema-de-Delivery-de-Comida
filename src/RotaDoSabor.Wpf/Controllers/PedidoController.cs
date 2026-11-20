namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Application.Simulacao;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Acompanhamento do pedido, cancelamento, repetição e avanço manual da simulação.</summary>
public sealed class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidos;
    private readonly ISimuladorStatusPedido _simulador;
    private readonly ISessaoUsuario _sessao;

    public PedidoController(IPedidoService pedidos, ISimuladorStatusPedido simulador, ISessaoUsuario sessao,
        IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _pedidos = pedidos;
        _simulador = simulador;
        _sessao = sessao;
    }

    public Task<DetalhePedidoDto?> CarregarAsync(int pedidoId) =>
        ExecutarAsync(() => _pedidos.ObterDetalheAsync(_sessao.ObterClienteIdObrigatorio(), pedidoId));

    /// <summary>NOVA FUNCIONALIDADE: cancelar pedido pendente.</summary>
    public async Task<bool> CancelarAsync(int pedidoId)
    {
        if (!Dialogo.Confirmar($"Deseja cancelar o pedido #{pedidoId}?", "Cancelar pedido"))
            return false;

        var sucesso = await ExecutarAsync(() => _pedidos.CancelarAsync(_sessao.ObterClienteIdObrigatorio(), pedidoId));
        if (sucesso)
            Dialogo.Informar($"Pedido #{pedidoId} cancelado.");
        return sucesso;
    }

    /// <summary>Botão de demonstração: avança o status sem esperar o temporizador da simulação.</summary>
    public async Task<bool> AvancarStatusAsync(int pedidoId)
    {
        StatusPedido? novoStatus = null;
        var sucesso = await ExecutarAsync(async () =>
            novoStatus = await _simulador.AvancarManualmenteAsync(_sessao.ObterClienteIdObrigatorio(), pedidoId));
        return sucesso && novoStatus is not null;
    }

    /// <summary>NOVA FUNCIONALIDADE: repetir pedido anterior (só itens disponíveis, com preço atual).</summary>
    public async Task RepetirAsync(int pedidoId)
    {
        if (!_sessao.Carrinho.EstaVazio &&
            !Dialogo.Confirmar("Seu carrinho atual será substituído pelos itens deste pedido. Continuar?", "Repetir pedido"))
            return;

        var resultado = await ExecutarAsync(() => _pedidos.RepetirAsync(_sessao.ObterClienteIdObrigatorio(), pedidoId, _sessao.Carrinho));
        if (resultado is null)
            return;

        if (resultado.ItensAdicionados == 0)
        {
            Dialogo.Avisar("Nenhum item deste pedido está disponível no momento.");
            return;
        }

        if (resultado.AlgumItemIndisponivel)
            Dialogo.Avisar("Itens indisponíveis que ficaram de fora:\n- " + string.Join("\n- ", resultado.ItensIndisponiveis));

        Navegador.Navegar(Tela.Carrinho);
    }

    public void Avaliar(int pedidoId) => Navegador.Navegar(Tela.Avaliacao, pedidoId);

    public void AbrirHistorico() => Navegador.Navegar(Tela.Historico);
}
