namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class CupomRepository : ICupomRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public CupomRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<Cupom?> ObterPorCodigoAsync(string codigo)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Cupons.AsNoTracking().FirstOrDefaultAsync(cupom => cupom.Codigo == codigo);
    }
}
