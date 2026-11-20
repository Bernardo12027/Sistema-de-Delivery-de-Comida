namespace RotaDoSabor.Domain.Interfaces;

/// <summary>
/// Itens que precisam de preparo na cozinha (ISP: só Prato implementa; Bebida não é obrigada).
/// </summary>
public interface IPreparacao
{
    int TempoPreparo { get; }

    /// <summary>Descreve o preparo do item (usado na simulação da cozinha).</summary>
    string Preparar();
}
