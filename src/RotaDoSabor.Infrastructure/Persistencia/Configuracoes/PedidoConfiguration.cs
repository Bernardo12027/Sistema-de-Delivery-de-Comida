namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

/// <summary>
/// Pedido referencia Cliente, Restaurante, Endereço e Cupom com DeleteBehavior.Restrict:
/// (1) nada apaga pedidos "por tabela" (histórico preservado) e
/// (2) evita o erro do SQL Server de múltiplos caminhos de cascata (Cliente→Endereço→Pedido e Cliente→Pedido).
/// Só a composição usa cascata: Pedido → ItensPedido e Pedido → Pagamento.
/// </summary>
public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");
        builder.HasKey(pedido => pedido.Id);

        builder.Property(pedido => pedido.DataHora).IsRequired();
        builder.Property(pedido => pedido.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(pedido => pedido.Subtotal).HasPrecision(10, 2);
        builder.Property(pedido => pedido.Desconto).HasPrecision(10, 2);
        builder.Property(pedido => pedido.TaxaEntrega).HasPrecision(10, 2);
        builder.Property(pedido => pedido.ValorTotal).HasPrecision(10, 2);
        builder.Property(pedido => pedido.TempoEstimadoMinutos);
        builder.Property(pedido => pedido.DataAtualizacaoStatus).IsRequired();
        builder.Property(pedido => pedido.RowVersion).IsRowVersion();

        // Índice que atende a consulta do histórico (WHERE ClienteId ORDER BY DataHora).
        builder.HasIndex(pedido => new { pedido.ClienteId, pedido.DataHora });

        builder.HasOne(pedido => pedido.Cliente)
            .WithMany(cliente => cliente.Pedidos)
            .HasForeignKey(pedido => pedido.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pedido => pedido.Restaurante)
            .WithMany(restaurante => restaurante.Pedidos)
            .HasForeignKey(pedido => pedido.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pedido => pedido.Endereco)
            .WithMany(endereco => endereco.Pedidos)
            .HasForeignKey(pedido => pedido.EnderecoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pedido => pedido.Cupom)
            .WithMany(cupom => cupom.Pedidos)
            .HasForeignKey(pedido => pedido.CupomId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Composição: itens não existem sem o pedido.
        builder.HasMany(pedido => pedido.Itens)
            .WithOne(item => item.Pedido)
            .HasForeignKey(item => item.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:1 Pedido — Pagamento (FK única em Pagamentos.PedidoId).
        builder.HasOne(pedido => pedido.Pagamento)
            .WithOne(pagamento => pagamento.Pedido)
            .HasForeignKey<Pagamento>(pagamento => pagamento.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1:0..1 Pedido — Avaliação (FK única garante "não avaliar duas vezes" também no banco).
        builder.HasOne(pedido => pedido.Avaliacao)
            .WithOne(avaliacao => avaliacao.Pedido)
            .HasForeignKey<Avaliacao>(avaliacao => avaliacao.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
