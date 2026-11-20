namespace RotaDoSabor.Infrastructure.Persistencia;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Usada SOMENTE em tempo de design pelas ferramentas do EF (Add-Migration / Update-Database).
/// Em tempo de execução a string de conexão vem do appsettings.json do projeto WPF.
/// </summary>
public sealed class RotaDoSaborDbContextFactory : IDesignTimeDbContextFactory<RotaDoSaborDbContext>
{
    public const string ConexaoPadraoLocalDb =
        "Server=(localdb)\\MSSQLLocalDB;Database=RotaDoSaborDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    public RotaDoSaborDbContext CreateDbContext(string[] args)
    {
        var conexao = Environment.GetEnvironmentVariable("ROTADOSABOR_CONNECTION") ?? ConexaoPadraoLocalDb;
        var opcoes = new DbContextOptionsBuilder<RotaDoSaborDbContext>()
            .UseSqlServer(conexao)
            .Options;
        return new RotaDoSaborDbContext(opcoes);
    }
}
