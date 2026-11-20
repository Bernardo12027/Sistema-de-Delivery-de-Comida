namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> builder)
    {
        builder.ToTable("Restaurantes");
        builder.HasKey(restaurante => restaurante.Id);

        builder.Property(restaurante => restaurante.Nome).IsRequired().HasMaxLength(Restaurante.TamanhoMaximoNome);
        builder.Property(restaurante => restaurante.Endereco).IsRequired().HasMaxLength(Restaurante.TamanhoMaximoEndereco);
        builder.Property(restaurante => restaurante.Telefone).IsRequired().HasMaxLength(Restaurante.TamanhoMaximoTelefone);
        builder.Property(restaurante => restaurante.AvaliacaoMedia).HasPrecision(3, 2);
        builder.Property(restaurante => restaurante.QuantidadeAvaliacoes);
        builder.Property(restaurante => restaurante.TempoEntregaMinutos);

        // Restrict: apagar uma categoria não pode apagar restaurantes.
        builder.HasOne(restaurante => restaurante.Categoria)
            .WithMany(categoria => categoria.Restaurantes)
            .HasForeignKey(restaurante => restaurante.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
