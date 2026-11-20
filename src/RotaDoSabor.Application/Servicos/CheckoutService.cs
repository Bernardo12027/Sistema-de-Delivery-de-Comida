namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Pagamentos;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;
using RotaDoSabor.Domain.Interfaces;

/// <summary>Caso de uso "Finalizar pedido" (inclui: gerenciar carrinho, selecionar endereço, realizar pagamento).</summary>
public interface ICheckoutService
{
    /// <summary>Tempo estimado = maior tempo de preparo entre os pratos + tempo de entrega do restaurante.</summary>
    Task<int> CalcularTempoEstimadoAsync(Carrinho carrinho);

    Task<PedidoFinalizadoDto> FinalizarAsync(int clienteId, Carrinho carrinho, FinalizarPedidoDto dados);
}

public sealed class CheckoutService : ICheckoutService
{
    /// <summary>Tempo mínimo de separação quando o pedido só tem bebidas.</summary>
    public const int TempoPreparoMinimoMinutos = 5;

    private readonly IPedidoRepository _pedidos;
    private readonly IProdutoRepository _produtos;
    private readonly IRestauranteRepository _restaurantes;
    private readonly IEnderecoRepository _enderecos;
    private readonly ICartaoRepository _cartoes;
    private readonly ICupomService _cupons;
    private readonly IProcessadorPagamento _processadorPagamento;
    private readonly IRelogio _relogio;

    public CheckoutService(IPedidoRepository pedidos, IProdutoRepository produtos, IRestauranteRepository restaurantes,
        IEnderecoRepository enderecos, ICartaoRepository cartoes, ICupomService cupons,
        IProcessadorPagamento processadorPagamento, IRelogio relogio)
    {
        _pedidos = pedidos;
        _produtos = produtos;
        _restaurantes = restaurantes;
        _enderecos = enderecos;
        _cartoes = cartoes;
        _cupons = cupons;
        _processadorPagamento = processadorPagamento;
        _relogio = relogio;
    }

    public async Task<int> CalcularTempoEstimadoAsync(Carrinho carrinho)
    {
        if (carrinho.EstaVazio)
            return 0;

        var produtos = await CarregarProdutosAsync(carrinho);
        var restaurante = await ObterRestauranteAsync(carrinho.RestauranteId!.Value);
        return CalcularTempoEstimado(produtos.Values, restaurante);
    }

    public async Task<PedidoFinalizadoDto> FinalizarAsync(int clienteId, Carrinho carrinho, FinalizarPedidoDto dados)
    {
        if (carrinho.EstaVazio)
            throw new RegraDeNegocioException("Adicione ao menos um item ao carrinho para finalizar o pedido.");

        var endereco = await ObterEnderecoDoClienteAsync(clienteId, dados.EnderecoId);
        var restaurante = await ObterRestauranteAsync(carrinho.RestauranteId!.Value);
        var produtos = await CarregarProdutosAsync(carrinho);
        var agora = _relogio.Agora;

        var pedido = new Pedido(clienteId, restaurante.Id, endereco.Id, agora, CalcularTempoEstimado(produtos.Values, restaurante));
        foreach (var item in carrinho.Itens)
        {
            if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                throw new RegraDeNegocioException($"O produto \"{item.NomeProduto}\" não existe mais no cardápio.");
            pedido.AdicionarItem(produto, item.Quantidade); // grava o preço atual (momento da compra)
        }

        if (!string.IsNullOrWhiteSpace(dados.CodigoCupom))
            pedido.AplicarCupom(await _cupons.ObterValidoAsync(dados.CodigoCupom), agora);

        var cartao = await ObterCartaoParaPagamentoAsync(clienteId, dados);
        var pagamento = new Pagamento(dados.FormaPagamento, pedido.ValorTotal, cartao?.Id);
        _processadorPagamento.Processar(pagamento, cartao);
        pedido.DefinirPagamento(pagamento);

        if (pagamento.Status == StatusPagamento.Recusado)
            pedido.Cancelar(agora);

        await _pedidos.AdicionarAsync(pedido);

        if (pagamento.Status != StatusPagamento.Recusado)
            carrinho.Limpar();

        return new PedidoFinalizadoDto(pedido.Id, pagamento.Status, MontarMensagem(pedido.Id, pagamento.Status));
    }

    private static int CalcularTempoEstimado(IEnumerable<Produto> produtos, Restaurante restaurante)
    {
        // Usa a interface IPreparacao: somente itens que têm preparo (pratos) influenciam o tempo da cozinha.
        var tempoPreparo = produtos.OfType<IPreparacao>()
            .Select(item => item.TempoPreparo)
            .DefaultIfEmpty(TempoPreparoMinimoMinutos)
            .Max();
        return tempoPreparo + restaurante.TempoEntregaMinutos;
    }

    private async Task<Dictionary<int, Produto>> CarregarProdutosAsync(Carrinho carrinho)
    {
        var ids = carrinho.Itens.Select(item => item.ProdutoId).ToList();
        var produtos = await _produtos.ListarPorIdsAsync(ids);
        return produtos.ToDictionary(produto => produto.Id);
    }

    private async Task<Restaurante> ObterRestauranteAsync(int restauranteId) =>
        await _restaurantes.ObterPorIdAsync(restauranteId) ?? throw new NaoEncontradoException("Restaurante");

    private async Task<Endereco> ObterEnderecoDoClienteAsync(int clienteId, int enderecoId)
    {
        var endereco = await _enderecos.ObterPorIdAsync(enderecoId);
        if (endereco is null || endereco.ClienteId != clienteId || !endereco.Ativo)
            throw new RegraDeNegocioException("Selecione um endereço de entrega válido.");
        return endereco;
    }

    private async Task<CartaoSimulado?> ObterCartaoParaPagamentoAsync(int clienteId, FinalizarPedidoDto dados)
    {
        if (dados.FormaPagamento is not (FormaPagamento.CartaoCredito or FormaPagamento.CartaoDebito))
            return null;

        if (dados.CartaoId is null)
            throw new RegraDeNegocioException("Selecione um cartão cadastrado.");

        var cartao = await _cartoes.ObterPorIdAsync(dados.CartaoId.Value);
        if (cartao is null || cartao.ClienteId != clienteId || !cartao.Ativo)
            throw new RegraDeNegocioException("Cartão inválido.");
        if (!cartao.AceitaForma(dados.FormaPagamento))
            throw new RegraDeNegocioException($"O cartão selecionado é de {cartao.Tipo.ParaTexto()}. Escolha a forma de pagamento correspondente.");

        return cartao;
    }

    private static string MontarMensagem(int pedidoId, StatusPagamento status) => status switch
    {
        StatusPagamento.Aprovado => $"Pedido #{pedidoId} realizado! Pagamento APROVADO (simulação).",
        StatusPagamento.Pendente => $"Pedido #{pedidoId} realizado! Pagamento em dinheiro na entrega (PENDENTE).",
        _ => $"Pagamento RECUSADO pela simulação. O pedido #{pedidoId} foi registrado como CANCELADO. Seu carrinho foi mantido: escolha outra forma de pagamento."
    };
}
