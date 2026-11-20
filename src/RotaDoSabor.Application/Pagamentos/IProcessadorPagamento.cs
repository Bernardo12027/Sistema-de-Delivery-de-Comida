namespace RotaDoSabor.Application.Pagamentos;

using RotaDoSabor.Domain.Entidades;

/// <summary>Representa o ator externo "Sistema de Pagamento Simulado" do diagrama de casos de uso.</summary>
public interface IProcessadorPagamento
{
    void Processar(Pagamento pagamento, CartaoSimulado? cartao);
}
