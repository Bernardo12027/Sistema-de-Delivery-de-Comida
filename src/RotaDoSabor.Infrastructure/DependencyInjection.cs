namespace RotaDoSabor.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Seguranca;
using RotaDoSabor.Infrastructure.Persistencia;
using RotaDoSabor.Infrastructure.Repositorios;
using RotaDoSabor.Infrastructure.Repositorios.Sql;
using RotaDoSabor.Infrastructure.Seguranca;

/// <summary>Liga as interfaces da Application às implementações concretas desta camada (DIP).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("String de conexão 'RotaDoSabor' não configurada no appsettings.json.");

        services.AddSingleton(new ConfiguracaoBanco(connectionString));

        services.AddDbContextFactory<RotaDoSaborDbContext>(opcoes => opcoes
            .UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(maxRetryCount: 3))
            // A migration inicial foi escrita junto com o código; se o snapshot divergir em algum detalhe
            // de metadados, o app continua subindo. Para conferir: dotnet ef migrations has-pending-model-changes
            .ConfigureWarnings(avisos => avisos.Ignore(RelationalEventId.PendingModelChangesWarning)));

        services.AddSingleton<IInicializadorBanco, InicializadorBanco>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

        services.AddTransient<IClienteRepository, ClienteRepository>();
        services.AddTransient<ICategoriaRepository, CategoriaRepository>();
        services.AddTransient<IRestauranteRepository, RestauranteRepository>();
        services.AddTransient<IProdutoRepository, ProdutoRepository>();
        services.AddTransient<IPedidoRepository, PedidoRepository>();
        services.AddTransient<IEnderecoRepository, EnderecoRepository>();
        services.AddTransient<ICartaoRepository, CartaoRepository>();
        services.AddTransient<ICupomRepository, CupomRepository>();
        services.AddTransient<IAvaliacaoRepository, AvaliacaoRepository>();

        // Consulta com SQL manual (ADO.NET).
        services.AddTransient<IHistoricoPedidoConsulta, HistoricoPedidoSqlRepository>();

        return services;
    }
}
