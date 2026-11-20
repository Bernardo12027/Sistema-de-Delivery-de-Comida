namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

/// <summary>Pagamento simulado do pedido (1:1). Nenhuma integração financeira real é feita.</summary>
public class Pagamento
{
    public const int TamanhoMaximoCodigoTransacao = 40;

    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public Pedido? Pedido { get; private set; }
    public decimal Valor { get; private set; }
    public FormaPagamento Forma { get; private set; }
    public StatusPagamento Status { get; private set; }
    public DateTime? DataPagamento { get; private set; }
    public string? CodigoTransacao { get; private set; }

    public int? CartaoSimuladoId { get; private set; }
    public CartaoSimulado? CartaoSimulado { get; private set; }

    protected Pagamento()
    {
    }

    public Pagamento(FormaPagamento forma, decimal valor, int? cartaoSimuladoId)
    {
        if (valor < 0)
            throw new RegraDeNegocioException("Valor do pagamento inválido.");

        var exigeCartao = forma is FormaPagamento.CartaoCredito or FormaPagamento.CartaoDebito;
        if (exigeCartao && cartaoSimuladoId is null)
            throw new RegraDeNegocioException("Selecione um cartão para pagar com cartão.");

        Forma = forma;
        Valor = valor;
        CartaoSimuladoId = exigeCartao ? cartaoSimuladoId : null;
        Status = StatusPagamento.Pendente;
    }

    public void Aprovar(DateTime data, string codigoTransacao)
    {
        GarantirPendente();
        Status = StatusPagamento.Aprovado;
        DataPagamento = data;
        CodigoTransacao = codigoTransacao;
    }

    public void Recusar(string codigoTransacao)
    {
        GarantirPendente();
        Status = StatusPagamento.Recusado;
        CodigoTransacao = codigoTransacao;
    }

    /// <summary>Pagamento em dinheiro fica PENDENTE até a entrega; aqui ele é confirmado.</summary>
    public void ConfirmarNaEntrega(DateTime data)
    {
        if (Status == StatusPagamento.Pendente)
        {
            Status = StatusPagamento.Aprovado;
            DataPagamento = data;
        }
    }

    /// <summary>Estorno simulado quando um pedido pendente é cancelado.</summary>
    public void Estornar()
    {
        if (Status == StatusPagamento.Aprovado)
            Status = StatusPagamento.Estornado;
        else if (Status == StatusPagamento.Pendente)
            Status = StatusPagamento.Recusado;
    }

    private void GarantirPendente()
    {
        if (Status != StatusPagamento.Pendente)
            throw new RegraDeNegocioException("Pagamento já processado.");
    }
}
