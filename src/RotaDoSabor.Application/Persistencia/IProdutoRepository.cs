namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> ListarPorRestauranteAsync(int restauranteId);
    Task<Produto?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<Produto>> ListarPorIdsAsync(IReadOnlyCollection<int> ids);
}
