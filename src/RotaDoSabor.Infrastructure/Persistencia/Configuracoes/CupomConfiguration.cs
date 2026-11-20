namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class CupomConfiguration : IEntityTypeConfiguration<Cupom>
{
    public void Configure(EntityTypeBuilder<Cupom> builder)
    {
        builder.ToTable("Cupons");
        builder.HasKey(cupom => cupom.Id);

        builder.Property(cupom => cupom.Codigo).IsRequired().HasMaxLength(Cupom.TamanhoMaximoCodigo);
        builder.Property(cupom => cupom.Descricao).IsRequired().HasMaxLength(Cupom.TamanhoMaximoDescricao);
        builder.Property(cupom => cupom.PercentualDesconto).HasPrecision(5, 2);
        builder.Property(cupom => cupom.DataValidade).HasColumnType("date");
        builder.Property(cupom => cupom.Ativo);

        builder.HasIndex(cupom => cupom.Codigo).IsUnique();
    }
}
