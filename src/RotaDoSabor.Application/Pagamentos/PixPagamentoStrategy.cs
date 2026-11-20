namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;

/// <summary>Pix simulado: sempre aprovado na hora (resultado padrão da simulação).</summary>
public sealed class PixPagamentoStrategy : IPagamentoStrategy
{
    public FormaPagamento Forma => FormaPagamento.Pix;

    public void Processar(Pagamento pagamento, CartaoSimulado? cartao, DateTime agora) =>
        pagamento.Aprovar(agora, CodigoTransacaoSimulada.Gerar("PIX"));
}
