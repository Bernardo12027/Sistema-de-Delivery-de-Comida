namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

/// <summary>ItemPedido é a tabela associativa do relacionamento N-N entre Pedido e Produto.</summary>
public sealed class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.NomeProduto).IsRequired().HasMaxLength(Produto.TamanhoMaximoNome);
        builder.Property(item => item.Quantidade);
        builder.Property(item => item.PrecoUnitario).HasPrecision(10, 2);
        builder.Property(item => item.Subtotal).HasPrecision(10, 2);

        builder.HasOne(item => item.Produto)
            .WithMany(produto => produto.ItensPedido)
            .HasForeignKey(item => item.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
