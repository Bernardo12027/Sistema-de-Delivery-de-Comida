namespace RotaDoSabor.Domain.Enums;

/// <summary>
/// Situação do pagamento simulado. "Estornado" é uma extensão usada quando um pedido
/// pendente com pagamento aprovado é cancelado pelo cliente.
/// </summary>
public enum StatusPagamento
{
    Pendente = 1,
    Aprovado = 2,
    Recusado = 3,
    Estornado = 4
}
