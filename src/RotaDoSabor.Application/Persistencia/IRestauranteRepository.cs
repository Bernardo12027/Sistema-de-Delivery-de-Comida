namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IRestauranteRepository
{
    /// <summary>Lista restaurantes filtrando por termo (nome do restaurante ou de produto) e categoria.</summary>
    Task<IReadOnlyList<Restaurante>> ListarAsync(string? termoBusca, int? categoriaId);
    Task<Restaurante?> ObterPorIdAsync(int id);
}
