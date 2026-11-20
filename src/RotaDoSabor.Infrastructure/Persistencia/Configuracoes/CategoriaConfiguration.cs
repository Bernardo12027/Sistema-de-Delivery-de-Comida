namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");
        builder.HasKey(categoria => categoria.Id);
        builder.Property(categoria => categoria.Nome).IsRequired().HasMaxLength(Categoria.TamanhoMaximoNome);
        builder.HasIndex(categoria => categoria.Nome).IsUnique();
    }
}
