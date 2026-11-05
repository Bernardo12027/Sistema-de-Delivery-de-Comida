using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Models;

namespace RotaDoSabor.Data;

/// <summary>
/// Contexto do Entity Framework (abordagem Code First): o esquema do banco
/// é gerado a partir das classes de <c>Models</c> + o mapeamento abaixo.
/// Novas entidades do backlog (Produto, Pedido, Endereço...) entram aqui como novos DbSet.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entidade =>
        {
            entidade.ToTable("Usuarios");
            entidade.HasKey(u => u.Id);

            // NOCASE: "Mestre" e "mestre" são o mesmo usuário (vale para buscas e para o índice único).
            entidade.Property(u => u.NomeUsuario)
                .IsRequired()
                .HasMaxLength(30)
                .UseCollation("NOCASE");

            entidade.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(120)
                .UseCollation("NOCASE");

            entidade.Property(u => u.SenhaHash).IsRequired();

            entidade.HasIndex(u => u.NomeUsuario).IsUnique();
            entidade.HasIndex(u => u.Email).IsUnique();
        });
    }
}
