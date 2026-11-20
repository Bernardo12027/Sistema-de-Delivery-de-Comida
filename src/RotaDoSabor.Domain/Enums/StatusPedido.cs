namespace RotaDoSabor.Domain.Enums;

/// <summary>
/// Ciclo de vida do pedido: PENDENTE → EM_PREPARO → SAIU_PARA_ENTREGA → ENTREGUE.
/// CANCELADO só é possível a partir de PENDENTE.
/// </summary>
public enum StatusPedido
{
    Pendente = 1,
    EmPreparo = 2,
    SaiuParaEntrega = 3,
    Entregue = 4,
    Cancelado = 5
}
