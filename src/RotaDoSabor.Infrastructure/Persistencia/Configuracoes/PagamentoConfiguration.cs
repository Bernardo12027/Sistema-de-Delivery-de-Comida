namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");
        builder.HasKey(pagamento => pagamento.Id);

        builder.Property(pagamento => pagamento.Valor).HasPrecision(10, 2);
        builder.Property(pagamento => pagamento.Forma).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(pagamento => pagamento.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(pagamento => pagamento.DataPagamento);
        builder.Property(pagamento => pagamento.CodigoTransacao).HasMaxLength(Pagamento.TamanhoMaximoCodigoTransacao);

        builder.HasOne(pagamento => pagamento.CartaoSimulado)
            .WithMany(cartao => cartao.Pagamentos)
            .HasForeignKey(pagamento => pagamento.CartaoSimuladoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
