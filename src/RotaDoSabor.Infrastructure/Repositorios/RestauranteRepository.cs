namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class RestauranteRepository : IRestauranteRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public RestauranteRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<IReadOnlyList<Restaurante>> ListarAsync(string? termoBusca, int? categoriaId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        IQueryable<Restaurante> consulta = db.Restaurantes.AsNoTracking().Include(restaurante => restaurante.Categoria);

        if (categoriaId is not null)
            consulta = consulta.Where(restaurante => restaurante.CategoriaId == categoriaId);

        if (!string.IsNullOrWhiteSpace(termoBusca))
            consulta = consulta.Where(restaurante =>
                restaurante.Nome.Contains(termoBusca) ||
                restaurante.Produtos.Any(produto => produto.Nome.Contains(termoBusca)));

        return await consulta
            .OrderByDescending(restaurante => restaurante.AvaliacaoMedia)
            .ThenBy(restaurante => restaurante.Nome)
            .ToListAsync();
    }

    public async Task<Restaurante?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Restaurantes.AsNoTracking()
            .Include(restaurante => restaurante.Categoria)
            .FirstOrDefaultAsync(restaurante => restaurante.Id == id);
    }
}
