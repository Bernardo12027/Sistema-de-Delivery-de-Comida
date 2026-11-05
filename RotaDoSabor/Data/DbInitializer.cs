using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Models;
using RotaDoSabor.Services;

namespace RotaDoSabor.Data;

/// <summary>
/// Cria o banco (se não existir) e garante o usuário "Mestre" (seed).
/// Roda uma vez na inicialização do aplicativo.
/// </summary>
public class DbInitializer
{
    private const string NomeMestre = "Mestre";
    private const string EmailMestre = "mestre@gmail.com";
    private const string SenhaMestre = "123456";

    private readonly IDbContextFactory<AppDbContext> _fabricaContexto;
    private readonly IPasswordHasher _hasher;

    public DbInitializer(IDbContextFactory<AppDbContext> fabricaContexto, IPasswordHasher hasher)
    {
        _fabricaContexto = fabricaContexto;
        _hasher = hasher;
    }

    public void Inicializar()
    {
        using var db = _fabricaContexto.CreateDbContext();

        CriarEsquema(db);
        CriarUsuarioMestre(db);
    }

    /// <summary>
    /// Se o projeto já tiver Migrations (Add-Migration), aplica-as; caso contrário cria o esquema
    /// direto a partir do modelo. Assim o app funciona agora e migra para Migrations sem mudar código.
    /// </summary>
    private static void CriarEsquema(AppDbContext db)
    {
        if (db.Database.GetMigrations().Any())
            db.Database.Migrate();
        else
            db.Database.EnsureCreated();
    }

    private void CriarUsuarioMestre(AppDbContext db)
    {
        var jaExiste = db.Usuarios.Any(u => u.Email == EmailMestre || u.NomeUsuario == NomeMestre);
        if (jaExiste)
            return;

        db.Usuarios.Add(new Usuario
        {
            NomeUsuario = NomeMestre,
            Email = EmailMestre,
            SenhaHash = _hasher.Hash(SenhaMestre)
        });
        db.SaveChanges();
    }
}
