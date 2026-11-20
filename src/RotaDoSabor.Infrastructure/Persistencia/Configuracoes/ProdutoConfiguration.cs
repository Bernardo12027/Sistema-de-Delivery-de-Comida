namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

/// <summary>
/// Herança TPH (Table per Hierarchy): Produto, Prato e Bebida ficam na tabela "Produtos",
/// diferenciados pela coluna discriminadora "TipoProduto".
/// </summary>
public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public const string ColunaDiscriminadora = "TipoProduto";

    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        builder.HasKey(produto => produto.Id);

        builder.HasDiscriminator<string>(ColunaDiscriminadora)
            .HasValue<Prato>("Prato")
            .HasValue<Bebida>("Bebida");
        builder.Property<string>(ColunaDiscriminadora).HasMaxLength(20);

        builder.Property(produto => produto.Nome).IsRequired().HasMaxLength(Produto.TamanhoMaximoNome);
        builder.Property(produto => produto.Descricao).IsRequired().HasMaxLength(Produto.TamanhoMaximoDescricao);
        builder.Property(produto => produto.Preco).HasPrecision(10, 2);
        builder.Property(produto => produto.Disponivel);

        // Restrict: preserva o histórico de pedidos (ItemPedido aponta para o produto).
        builder.HasOne(produto => produto.Restaurante)
            .WithMany(restaurante => restaurante.Produtos)
            .HasForeignKey(produto => produto.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(produto => produto.Categoria)
            .WithMany(categoria => categoria.Produtos)
            .HasForeignKey(produto => produto.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class PratoConfiguration : IEntityTypeConfiguration<Prato>
{
    public void Configure(EntityTypeBuilder<Prato> builder)
    {
        builder.Property(prato => prato.TempoPreparo);
        builder.Property(prato => prato.Tamanho).HasMaxLength(Prato.TamanhoMaximoTamanho);
    }
}

public sealed class BebidaConfiguration : IEntityTypeConfiguration<Bebida>
{
    public void Configure(EntityTypeBuilder<Bebida> builder)
    {
        builder.Property(bebida => bebida.Volume);
        builder.Property(bebida => bebida.Tipo).HasMaxLength(Bebida.TamanhoMaximoTipo);
    }
}
