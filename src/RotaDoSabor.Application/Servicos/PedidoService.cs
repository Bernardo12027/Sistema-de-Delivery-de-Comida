namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

/// <summary>Consulta e ações sobre pedidos já finalizados.</summary>
public interface IPedidoService
{
    Task<DetalhePedidoDto> ObterDetalheAsync(int clienteId, int pedidoId);

    /// <summary>NOVA FUNCIONALIDADE: cancelar pedido pendente.</summary>
    Task CancelarAsync(int clienteId, int pedidoId);

    /// <summary>NOVA FUNCIONALIDADE: repetir pedido — refaz o carrinho com os itens ainda disponíveis (preço atual).</summary>
    Task<RepeticaoPedidoDto> RepetirAsync(int clienteId, int pedidoId, Carrinho carrinho);
}

public sealed class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProdutoRepository _produtos;
    private readonly IRestauranteRepository _restaurantes;
    private readonly IRelogio _relogio;

    public PedidoService(IPedidoRepository pedidos, IProdutoRepository produtos, IRestauranteRepository restaurantes, IRelogio relogio)
    {
        _pedidos = pedidos;
        _produtos = produtos;
        _restaurantes = restaurantes;
        _relogio = relogio;
    }

    public async Task<DetalhePedidoDto> ObterDetalheAsync(int clienteId, int pedidoId)
    {
        var pedido = GarantirDono(await _pedidos.ObterDetalheAsync(pedidoId), clienteId);

        return new DetalhePedidoDto(
            pedido.Id,
            pedido.DataHora,
            pedido.RestauranteId,
            pedido.Restaurante?.Nome ?? string.Empty,
            pedido.Status,
            pedido.Subtotal,
            pedido.Desconto,
            pedido.TaxaEntrega,
            pedido.ValorTotal,
            pedido.Endereco?.ObterDescricao() ?? string.Empty,
            pedido.Pagamento?.Forma ?? FormaPagamento.Pix,
            pedido.Pagamento?.Status ?? StatusPagamento.Pendente,
            pedido.Cupom?.Codigo,
            pedido.TempoEstimadoMinutos,
            pedido.ObterPrevisaoEntrega(),
            pedido.Itens.Select(item => item.ParaDto()).ToList(),
            pedido.PodeSerCancelado(),
            pedido.PodeSerAvaliado());
    }

    public async Task CancelarAsync(int clienteId, int pedidoId)
    {
        var pedido = GarantirDono(await _pedidos.ObterPorIdAsync(pedidoId), clienteId);
        pedido.Cancelar(_relogio.Agora);

        if (!await _pedidos.AtualizarAsync(pedido))
            throw new RegraDeNegocioException("O status do pedido acabou de mudar e ele não pode mais ser cancelado. Atualize a tela.");
    }

    public async Task<RepeticaoPedidoDto> RepetirAsync(int clienteId, int pedidoId, Carrinho carrinho)
    {
        var pedido = GarantirDono(await _pedidos.ObterPorIdAsync(pedidoId), clienteId);
        var restaurante = await _restaurantes.ObterPorIdAsync(pedido.RestauranteId) ?? throw new NaoEncontradoException("Restaurante");
        var produtos = (await _produtos.ListarPorIdsAsync(pedido.Itens.Select(item => item.ProdutoId).ToList()))
            .ToDictionary(produto => produto.Id);

        carrinho.Limpar();
        var indisponiveis = new List<string>();
        var adicionados = 0;

        foreach (var item in pedido.Itens)
        {
            if (produtos.TryGetValue(item.ProdutoId, out var produto) && produto.Disponivel)
            {
                carrinho.Adicionar(produto, restaurante.Nome, item.Quantidade);
                adicionados++;
            }
            else
            {
                indisponiveis.Add(item.NomeProduto);
            }
        }

        return new RepeticaoPedidoDto(adicionados, indisponiveis);
    }

    private static Pedido GarantirDono(Pedido? pedido, int clienteId)
    {
        if (pedido is null || pedido.ClienteId != clienteId)
            throw new NaoEncontradoException("Pedido");
        return pedido;
    }
}
