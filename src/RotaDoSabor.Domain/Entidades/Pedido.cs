namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Pedido finalizado. Compõe seus itens (composição Pedido ◆→ ItemPedido) e possui um único pagamento (1:1).
/// Todas as transições de status passam por métodos desta classe, garantindo as regras de negócio.
/// </summary>
public class Pedido
{
    public int Id { get; private set; }
    public DateTime DataHora { get; private set; }
    public StatusPedido Status { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Desconto { get; private set; }
    public decimal TaxaEntrega { get; private set; }
    public decimal ValorTotal { get; private set; }
    public int TempoEstimadoMinutos { get; private set; }
    public DateTime DataAtualizacaoStatus { get; private set; }

    /// <summary>Controle de concorrência otimista (evita que a simulação sobrescreva um cancelamento).</summary>
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public int ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }

    public int RestauranteId { get; private set; }
    public Restaurante? Restaurante { get; private set; }

    public int EnderecoId { get; private set; }
    public Endereco? Endereco { get; private set; }

    public int? CupomId { get; private set; }
    public Cupom? Cupom { get; private set; }

    public ICollection<ItemPedido> Itens { get; private set; } = new List<ItemPedido>();
    public Pagamento? Pagamento { get; private set; }
    public Avaliacao? Avaliacao { get; private set; }

    protected Pedido()
    {
    }

    public Pedido(int clienteId, int restauranteId, int enderecoId, DateTime dataHora, int tempoEstimadoMinutos)
    {
        ClienteId = Guarda.IdValido(clienteId, "Cliente");
        RestauranteId = Guarda.IdValido(restauranteId, "Restaurante");
        EnderecoId = Guarda.IdValido(enderecoId, "Endereço");
        DataHora = dataHora;
        DataAtualizacaoStatus = dataHora;
        TempoEstimadoMinutos = tempoEstimadoMinutos > 0 ? tempoEstimadoMinutos : 1;
        Status = StatusPedido.Pendente;
        TaxaEntrega = 0m; // Regra do MVP: entrega gratuita.
    }

    /// <summary>Adiciona um item gravando o preço do produto no momento da compra.</summary>
    public void AdicionarItem(Produto produto, int quantidade)
    {
        if (!produto.Disponivel)
            throw new RegraDeNegocioException($"O produto \"{produto.Nome}\" está indisponível.");
        if (produto.RestauranteId != RestauranteId)
            throw new RegraDeNegocioException("Todos os itens do pedido devem ser do mesmo restaurante.");

        Itens.Add(new ItemPedido(produto.Id, produto.Nome, quantidade, produto.Preco));
        RecalcularTotais();
    }

    public void AplicarCupom(Cupom cupom, DateTime agora)
    {
        if (Itens.Count == 0)
            throw new RegraDeNegocioException("Adicione itens antes de aplicar um cupom.");
        if (!cupom.EstaValido(agora))
            throw new RegraDeNegocioException("Cupom inválido ou expirado.");

        CupomId = cupom.Id;
        Desconto = cupom.CalcularDesconto(Subtotal);
        RecalcularTotais();
    }

    public void DefinirPagamento(Pagamento pagamento)
    {
        if (Pagamento is not null)
            throw new RegraDeNegocioException("O pedido já possui pagamento.");
        Pagamento = pagamento;
    }

    /// <summary>NOVA FUNCIONALIDADE: cancelar pedido — permitido somente enquanto PENDENTE.</summary>
    public void Cancelar(DateTime agora)
    {
        if (!PodeSerCancelado())
            throw new RegraDeNegocioException("Somente pedidos com status PENDENTE podem ser cancelados.");

        Status = StatusPedido.Cancelado;
        DataAtualizacaoStatus = agora;
        Pagamento?.Estornar();
    }

    /// <summary>Avança para a próxima etapa. Pedidos cancelados ou entregues não avançam.</summary>
    public void AvancarStatus(DateTime agora)
    {
        var proximo = ObterProximoStatus()
            ?? throw new RegraDeNegocioException($"Pedido com status {Status.ParaTexto()} não pode avançar.");

        Status = proximo;
        DataAtualizacaoStatus = agora;

        if (Status == StatusPedido.Entregue)
            Pagamento?.ConfirmarNaEntrega(agora);
    }

    public StatusPedido? ObterProximoStatus() => Status switch
    {
        StatusPedido.Pendente => StatusPedido.EmPreparo,
        StatusPedido.EmPreparo => StatusPedido.SaiuParaEntrega,
        StatusPedido.SaiuParaEntrega => StatusPedido.Entregue,
        _ => null
    };

    public bool PodeSerCancelado() => Status == StatusPedido.Pendente;

    public bool EstaEmAndamento() => ObterProximoStatus() is not null;

    public bool PodeSerAvaliado() => Status == StatusPedido.Entregue && Avaliacao is null;

    public DateTime ObterPrevisaoEntrega() => DataHora.AddMinutes(TempoEstimadoMinutos);

    private void RecalcularTotais()
    {
        Subtotal = Itens.Sum(item => item.Subtotal);
        Desconto = Math.Min(Desconto, Subtotal); // desconto nunca maior que o total
        ValorTotal = Subtotal - Desconto + TaxaEntrega;
    }
}
