namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class AvaliacaoRepository : IAvaliacaoRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public AvaliacaoRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task RegistrarAsync(Avaliacao avaliacao, Restaurante restauranteComNovaMedia)
    {
        await using var db = await _fabrica.CreateDbContextAsync();

        db.Avaliacoes.Add(avaliacao);

        // Atualiza só as colunas da média (o restante do restaurante não muda).
        db.Restaurantes.Attach(restauranteComNovaMedia);
        var entrada = db.Entry(restauranteComNovaMedia);
        entrada.Property(restaurante => restaurante.AvaliacaoMedia).IsModified = true;
        entrada.Property(restaurante => restaurante.QuantidadeAvaliacoes).IsModified = true;

        await db.SaveChangesAsync(); // avaliação + média na mesma transação
    }
}
