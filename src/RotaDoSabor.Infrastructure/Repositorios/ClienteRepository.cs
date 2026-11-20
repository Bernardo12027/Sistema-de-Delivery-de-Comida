namespace RotaDoSabor.Infrastructure.Repositorios;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Infrastructure.Persistencia;

/// <summary>
/// Cada método cria seu próprio DbContext (IDbContextFactory), padrão recomendado para aplicações desktop:
/// evita contexto "vivo" por muito tempo e permite uso seguro pela simulação em segundo plano.
/// </summary>
public sealed class ClienteRepository : IClienteRepository
{
    private readonly IDbContextFactory<RotaDoSaborDbContext> _fabrica;

    public ClienteRepository(IDbContextFactory<RotaDoSaborDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<Cliente?> ObterPorIdAsync(int id)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Clientes.AsNoTracking().FirstOrDefaultAsync(cliente => cliente.Id == id);
    }

    public async Task<Cliente?> ObterPorEmailAsync(string email)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Clientes.AsNoTracking().FirstOrDefaultAsync(cliente => cliente.Email == email);
    }

    public async Task<Cliente?> ObterPorNomeUsuarioAsync(string nomeUsuario)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Clientes.AsNoTracking().FirstOrDefaultAsync(cliente => cliente.NomeUsuario == nomeUsuario);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Clientes.AnyAsync(cliente => cliente.Email == email);
    }

    public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        return await db.Clientes.AnyAsync(cliente => cliente.NomeUsuario == nomeUsuario);
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        await using var db = await _fabrica.CreateDbContextAsync();
        db.Clientes.Update(cliente);
        await db.SaveChangesAsync();
    }
}
