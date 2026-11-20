namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IAvaliacaoRepository
{
    /// <summary>Salva a avaliação e a nova média do restaurante na mesma transação.</summary>
    Task RegistrarAsync(Avaliacao avaliacao, Restaurante restauranteComNovaMedia);
}
