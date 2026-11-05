using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Data;
using RotaDoSabor.Models;

namespace RotaDoSabor.Repositories;

/// <summary>
/// Persistência de usuários com EF Core. Cada operação usa um DbContext curto
/// (criado pela fábrica), o padrão recomendado para aplicações desktop.
/// SOLID (S): esta classe só cuida de acesso a dados.
/// </summary>
public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbContextFactory<AppDbContext> _fabricaContexto;

    public UsuarioRepository(IDbContextFactory<AppDbContext> fabricaContexto)
    {
        _fabricaContexto = fabricaContexto;
    }

    /// <summary>
    /// FUNCIONALIDADE COM SQL PURO (exigência da atividade: EF + ao menos uma consulta via SQL).
    /// A consulta é parametrizada ({identificador} vira parâmetro), então não há risco de SQL Injection.
    /// A comparação ignora maiúsculas/minúsculas porque as colunas usam collation NOCASE.
    /// </summary>
    public async Task<Usuario?> ObterPorNomeOuEmailAsync(string identificador)
    {
        await using var db = await _fabricaContexto.CreateDbContextAsync();

        return await db.Usuarios
            .FromSqlInterpolated($"SELECT * FROM Usuarios WHERE NomeUsuario = {identificador} OR Email = {identificador}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario)
    {
        await using var db = await _fabricaContexto.CreateDbContextAsync();
        return await db.Usuarios.AnyAsync(u => u.NomeUsuario == nomeUsuario);
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        await using var db = await _fabricaContexto.CreateDbContextAsync();
        return await db.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task AdicionarAsync(Usuario usuario)
    {
        await using var db = await _fabricaContexto.CreateDbContextAsync();
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
    }
}
