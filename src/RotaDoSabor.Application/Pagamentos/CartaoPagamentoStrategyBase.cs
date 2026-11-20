namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;

/// <summary>
/// Regras comuns de cartão (crédito e débito) — evita duplicação.
/// Simulação: aprova por padrão; cartão com final "0000" é recusado para demonstrar o status RECUSADO.
/// </summary>
public abstract class CartaoPagamentoStrategyBase : IPagamentoStrategy
{
    public const string FinalCartaoRecusado = "0000";

    public abstract FormaPagamento Forma { get; }

    protected abstract string PrefixoTransacao { get; }

    public void Processar(Pagamento pagamento, CartaoSimulado? cartao, DateTime agora)
    {
        var codigo = CodigoTransacaoSimulada.Gerar(PrefixoTransacao);

        if (cartao is null || !cartao.Ativo || !cartao.AceitaForma(Forma) || cartao.UltimosDigitos == FinalCartaoRecusado)
        {
            pagamento.Recusar(codigo);
            return;
        }

        pagamento.Aprovar(agora, codigo);
    }
}

public sealed class CartaoCreditoPagamentoStrategy : CartaoPagamentoStrategyBase
{
    public override FormaPagamento Forma => FormaPagamento.CartaoCredito;
    protected override string PrefixoTransacao => "CRED";
}

public sealed class CartaoDebitoPagamentoStrategy : CartaoPagamentoStrategyBase
{
    public override FormaPagamento Forma => FormaPagamento.CartaoDebito;
    protected override string PrefixoTransacao => "DEB";
}
