namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;

/// <summary>
/// Strategy de pagamento (OCP): para criar uma nova forma de pagamento basta criar uma nova classe
/// que implemente esta interface e registrá-la na injeção de dependência — nada existente é alterado.
/// </summary>
public interface IPagamentoStrategy
{
    FormaPagamento Forma { get; }

    void Processar(Pagamento pagamento, CartaoSimulado? cartao, DateTime agora);
}
