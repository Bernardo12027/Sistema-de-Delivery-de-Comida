namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;

/// <summary>Dinheiro: pago na entrega. Fica PENDENTE e é aprovado quando o pedido chega a ENTREGUE.</summary>
public sealed class DinheiroPagamentoStrategy : IPagamentoStrategy
{
    public FormaPagamento Forma => FormaPagamento.Dinheiro;

    public void Processar(Pagamento pagamento, CartaoSimulado? cartao, DateTime agora)
    {
        // Nada a processar agora: o status permanece PENDENTE até a entrega.
    }
}
