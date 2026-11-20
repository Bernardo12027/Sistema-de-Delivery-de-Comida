namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Infrastructure.Persistencia;

public sealed class PedidoRepository : IPedidoRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public PedidoRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Pedidos.Add(pedido); // grava Pedido + ItensPedido + Pagamento em uma única transação
        await db.SaveChangesAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Pedidos.AsNoTracking()
            .Include(pedido => pedido.Itens)
            .Include(pedido => pedido.Pagamento)
            .Include(pedido => pedido.Avaliacao)
            .FirstOrDefaultAsync(pedido => pedido.Id == id);
    }

    public async Task<Pedido?> ObterDetalheAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Pedidos.AsNoTracking()
            .Include(pedido => pedido.Itens)
            .Include(pedido => pedido.Pagamento)
            .Include(pedido => pedido.Avaliacao)
            .Include(pedido => pedido.Restaurante)
            .Include(pedido => pedido.Endereco)
            .Include(pedido => pedido.Cupom)
            .FirstOrDefaultAsync(pedido => pedido.Id == id);
    }

    public async Task<IReadOnlyList<Pedido>> ListarEmAndamentoAsync()
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Pedidos.AsNoTracking()
            .Include(pedido => pedido.Pagamento)
            .Where(pedido => pedido.Status != StatusPedido.Cancelado && pedido.Status != StatusPedido.Entregue)
            .ToListAsync();
    }

    public async Task<bool> AtualizarAsync(Pedido pedido)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        var existente = await db.Pedidos
            .Include(registro => registro.Pagamento)
            .FirstOrDefaultAsync(registro => registro.Id == pedido.Id);

        if (existente is null)
            return false;

        var entrada = db.Entry(existente);
        // A versão ORIGINAL passa a ser a que foi lida antes da alteração: se outro processo mudou o pedido
        // nesse meio tempo, o UPDATE ... WHERE RowVersion = @original não encontra a linha (concorrência otimista).
        entrada.Property(registro => registro.RowVersion).OriginalValue = pedido.RowVersion;
        entrada.CurrentValues.SetValues(pedido);
        entrada.Property(registro => registro.RowVersion).IsModified = false;

        if (pedido.Pagamento is not null && existente.Pagamento is not null)
            db.Entry(existente.Pagamento).CurrentValues.SetValues(pedido.Pagamento);

        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
    }
}
