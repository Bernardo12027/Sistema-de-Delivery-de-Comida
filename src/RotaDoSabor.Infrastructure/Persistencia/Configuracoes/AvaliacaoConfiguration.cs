namespace RotaDoSabor.Infrastructure.Persistencia.Configuracoes;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RotaDoSabor.Domain.Entidades;

public sealed class AvaliacaoConfiguration : IEntityTypeConfiguration<Avaliacao>
{
    public void Configure(EntityTypeBuilder<Avaliacao> builder)
    {
        builder.ToTable("Avaliacoes", tabela => tabela.HasCheckConstraint("CK_Avaliacoes_Nota", "[Nota] BETWEEN 1 AND 5"));
        builder.HasKey(avaliacao => avaliacao.Id);

        builder.Property(avaliacao => avaliacao.Nota);
        builder.Property(avaliacao => avaliacao.Comentario).HasMaxLength(Avaliacao.TamanhoMaximoComentario);
        builder.Property(avaliacao => avaliacao.DataAvaliacao).IsRequired();

        builder.HasOne(avaliacao => avaliacao.Cliente)
            .WithMany(cliente => cliente.Avaliacoes)
            .HasForeignKey(avaliacao => avaliacao.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(avaliacao => avaliacao.Restaurante)
            .WithMany(restaurante => restaurante.Avaliacoes)
            .HasForeignKey(avaliacao => avaliacao.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
