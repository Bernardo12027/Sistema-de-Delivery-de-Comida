namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(cliente => cliente.Id);

        builder.Property(cliente => cliente.Nome).IsRequired().HasMaxLength(Cliente.TamanhoMaximoNome);
        builder.Property(cliente => cliente.Email).IsRequired().HasMaxLength(Cliente.TamanhoMaximoEmail);
        builder.Property(cliente => cliente.Telefone).IsRequired().HasMaxLength(Cliente.TamanhoMaximoTelefone);
        builder.Property(cliente => cliente.NomeUsuario).IsRequired().HasMaxLength(Cliente.TamanhoMaximoNomeUsuario);
        builder.Property(cliente => cliente.SenhaHash).IsRequired().HasMaxLength(Cliente.TamanhoMaximoSenhaHash);
        builder.Property(cliente => cliente.DataCadastro).IsRequired();

        // E-mail e nome de usuário únicos (também validados no serviço de cadastro).
        builder.HasIndex(cliente => cliente.Email).IsUnique();
        builder.HasIndex(cliente => cliente.NomeUsuario).IsUnique();
    }
}
