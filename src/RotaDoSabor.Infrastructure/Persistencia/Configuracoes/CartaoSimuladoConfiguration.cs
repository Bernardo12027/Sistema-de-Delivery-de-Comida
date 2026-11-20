namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

/// <summary>Não existem colunas para número completo, validade ou CVV — por projeto, esses dados não são armazenados.</summary>
public sealed class CartaoSimuladoConfiguration : IEntityTypeConfiguration<CartaoSimulado>
{
    public void Configure(EntityTypeBuilder<CartaoSimulado> builder)
    {
        builder.ToTable("CartoesSimulados");
        builder.HasKey(cartao => cartao.Id);

        builder.Property(cartao => cartao.IdentificadorFicticio).IsRequired().HasMaxLength(CartaoSimulado.TamanhoMaximoIdentificador);
        builder.Property(cartao => cartao.Bandeira).IsRequired().HasMaxLength(CartaoSimulado.TamanhoMaximoBandeira);
        builder.Property(cartao => cartao.UltimosDigitos).IsRequired().HasMaxLength(CartaoSimulado.QuantidadeUltimosDigitos);
        builder.Property(cartao => cartao.Tipo).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(cartao => cartao.Apelido).IsRequired().HasMaxLength(CartaoSimulado.TamanhoMaximoApelido);
        builder.Property(cartao => cartao.Ativo);
        builder.Property(cartao => cartao.DataCadastro).IsRequired();

        builder.HasIndex(cartao => cartao.IdentificadorFicticio).IsUnique();

        builder.HasOne(cartao => cartao.Cliente)
            .WithMany(cliente => cliente.Cartoes)
            .HasForeignKey(cartao => cartao.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
