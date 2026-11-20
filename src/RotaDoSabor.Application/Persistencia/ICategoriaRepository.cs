namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface ICategoriaRepository
{
    /// <summary>Categorias que possuem ao menos um restaurante (usadas nos filtros da tela inicial).</summary>
    Task<IReadOnlyList<Categoria>> ListarComRestaurantesAsync();
}
