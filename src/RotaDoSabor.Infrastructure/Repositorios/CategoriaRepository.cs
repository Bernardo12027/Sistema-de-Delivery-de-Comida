namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class CategoriaRepository : ICategoriaRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public CategoriaRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<IReadOnlyList<Categoria>> ListarComRestaurantesAsync()
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Categorias.AsNoTracking()
            .Where(categoria => categoria.Restaurantes.Any())
            .OrderBy(categoria => categoria.Id)
            .ToListAsync();
    }
}
