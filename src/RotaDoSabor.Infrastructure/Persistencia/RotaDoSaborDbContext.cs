namespace RotaDoSabor.Infrastructure.Persistencia;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Domain.Entidades;

/// <summary>
/// Contexto do EF Core (Code First). O mapeamento de cada entidade fica em uma classe própria
/// em Persistencia/Configuracoes (IEntityTypeConfiguration), aplicadas automaticamente abaixo.
/// </summary>
public class RotaDoSaborDbContext : DbContext
{
    public RotaDoSaborDbContext(DbContextOptions<RotaDoSaborDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Prato> Pratos => Set<Prato>();
    public DbSet<Bebida> Bebidas => Set<Bebida>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<ItemPedido> ItensPedido => Set<ItemPedido>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();
    public DbSet<Cupom> Cupons => Set<Cupom>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<CartaoSimulado> CartoesSimulados => Set<CartaoSimulado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RotaDoSaborDbContext).Assembly);
    }
}
