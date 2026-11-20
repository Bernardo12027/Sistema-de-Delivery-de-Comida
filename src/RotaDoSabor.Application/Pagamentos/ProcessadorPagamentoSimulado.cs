namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

/// <summary>Escolhe a strategy correspondente à forma de pagamento. Não conhece as classes concretas (DIP).</summary>
public sealed class ProcessadorPagamentoSimulado : IProcessadorPagamento
{
    private readonly IReadOnlyDictionary<FormaPagamento, IPagamentoStrategy> _estrategias;
    private readonly IRelogio _relogio;

    public ProcessadorPagamentoSimulado(IEnumerable<IPagamentoStrategy> estrategias, IRelogio relogio)
    {
        _estrategias = estrategias.ToDictionary(estrategia => estrategia.Forma);
        _relogio = relogio;
    }

    public void Processar(Pagamento pagamento, CartaoSimulado? cartao)
    {
        if (!_estrategias.TryGetValue(pagamento.Forma, out var estrategia))
            throw new RegraDeNegocioException($"Forma de pagamento {pagamento.Forma.ParaTexto()} não suportada.");

        estrategia.Processar(pagamento, cartao, _relogio.Agora);
    }
}
