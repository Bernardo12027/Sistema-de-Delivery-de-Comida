namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Enderecos");
        builder.HasKey(endereco => endereco.Id);

        builder.Property(endereco => endereco.Logradouro).IsRequired().HasMaxLength(Endereco.TamanhoMaximoLogradouro);
        builder.Property(endereco => endereco.Numero).IsRequired().HasMaxLength(Endereco.TamanhoMaximoNumero);
        builder.Property(endereco => endereco.Complemento).HasMaxLength(Endereco.TamanhoMaximoComplemento);
        builder.Property(endereco => endereco.Bairro).IsRequired().HasMaxLength(Endereco.TamanhoMaximoBairro);
        builder.Property(endereco => endereco.Cidade).IsRequired().HasMaxLength(Endereco.TamanhoMaximoCidade);
        builder.Property(endereco => endereco.Cep).IsRequired().HasMaxLength(Endereco.TamanhoCep);
        builder.Property(endereco => endereco.Padrao);
        builder.Property(endereco => endereco.Ativo);

        builder.HasOne(endereco => endereco.Cliente)
            .WithMany(cliente => cliente.Enderecos)
            .HasForeignKey(endereco => endereco.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
