namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class CartaoRepository : ICartaoRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public CartaoRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<IReadOnlyList<CartaoSimulado>> ListarAtivosPorClienteAsync(int clienteId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.CartoesSimulados.AsNoTracking()
            .Where(cartao => cartao.ClienteId == clienteId && cartao.Ativo)
            .OrderBy(cartao => cartao.DataCadastro)
            .ToListAsync();
    }

    public async Task<CartaoSimulado?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.CartoesSimulados.AsNoTracking().FirstOrDefaultAsync(cartao => cartao.Id == id);
    }

    public async Task AdicionarAsync(CartaoSimulado cartao)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.CartoesSimulados.Add(cartao);
        await db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(CartaoSimulado cartao)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.CartoesSimulados.Update(cartao);
        await db.SaveChangesAsync();
    }

    public async Task RemoverAsync(CartaoSimulado cartao)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.CartoesSimulados.Remove(cartao);
        await db.SaveChangesAsync();
    }

    public async Task<bool> PossuiPagamentosAsync(int cartaoId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Pagamentos.AnyAsync(pagamento => pagamento.CartaoSimuladoId == cartaoId);
    }
}
