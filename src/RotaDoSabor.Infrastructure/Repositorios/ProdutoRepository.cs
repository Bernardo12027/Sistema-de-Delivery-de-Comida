namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

/// <summary>Consultas sobre a hierarquia Produto (TPH): o EF devolve instâncias de Prato ou Bebida.</summary>
public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public ProdutoRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<IReadOnlyList<Produto>> ListarPorRestauranteAsync(int restauranteId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Produtos.AsNoTracking()
            .Include(produto => produto.Categoria)
            .Where(produto => produto.RestauranteId == restauranteId)
            .ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Produtos.AsNoTracking()
            .Include(produto => produto.Restaurante)
            .FirstOrDefaultAsync(produto => produto.Id == id);
    }

    public async Task<IReadOnlyList<Produto>> ListarPorIdsAsync(IReadOnlyCollection<int> ids)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Produtos.AsNoTracking()
            .Where(produto => ids.Contains(produto.Id))
            .ToListAsync();
    }
}
