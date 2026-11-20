namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class EnderecoRepository : IEnderecoRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public EnderecoRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<IReadOnlyList<Endereco>> ListarAtivosPorClienteAsync(int clienteId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Enderecos.AsNoTracking()
            .Where(endereco => endereco.ClienteId == clienteId && endereco.Ativo)
            .ToListAsync();
    }

    public async Task<Endereco?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Enderecos.AsNoTracking().FirstOrDefaultAsync(endereco => endereco.Id == id);
    }

    public async Task AdicionarAsync(Endereco endereco)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Enderecos.Add(endereco);
        await db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(IEnumerable<Endereco> enderecos)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Enderecos.UpdateRange(enderecos);
        await db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Endereco endereco)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Enderecos.Remove(endereco);
        await db.SaveChangesAsync();
    }

    public async Task<bool> PossuiPedidosAsync(int enderecoId)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Pedidos.AnyAsync(pedido => pedido.EnderecoId == enderecoId);
    }
}
