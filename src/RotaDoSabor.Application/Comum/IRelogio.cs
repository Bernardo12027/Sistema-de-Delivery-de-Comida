namespace RotaDoSabor.Application.Comum;

/// <summary>Abstração do relógio: permite testar regras de data/hora (cupom, simulação) sem esperar.</summary>
public interface IRelogio
{
    DateTime Agora { get; }
}
