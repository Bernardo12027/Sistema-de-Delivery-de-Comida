namespace RotaDoSabor.Infrastructure.Persistencia;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Infrastructure.Persistencia.Seed;

public interface IInicializadorBanco
{
    /// <summary>Aplica as migrations pendentes (cria o banco se não existir) e popula os dados iniciais.</summary>
    Task InicializarAsync(CancellationToken cancellationToken = default);
}

public sealed class InicializadorBanco : IInicializadorBanco
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public InicializadorBanco(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task InicializarAsync(CancellationToken cancellationToken = default)
    {
        await using var db = await _fabrica.CreateDbContextAsync(cancellationToken);
        await db.Database.MigrateAsync(cancellationToken);
        await DadosIniciais.PopularAsync(db, cancellationToken);
    }
}
