namespace RotaDoSabor.Wpf.Configuracao;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Application;
using RotaDoSabor.Application.Simulacao;
using RotaDoSabor.Infrastructure;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;
using RotaDoSabor.Wpf.Views;

/// <summary>
/// Composition root: único lugar que conhece todas as implementações concretas (DIP).
/// Lê o appsettings.json e registra Application, Infrastructure, controllers e views.
/// </summary>
public static class CompositionRoot
{
    public const string NomeConexao = "RotaDoSabor";
    public const string SecaoSimulacao = "SimulacaoPedido";

    public static ServiceProvider CriarServicos()
    {
        var configuracao = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var conexao = configuracao.GetConnectionString(NomeConexao) ?? string.Empty;
        var simulacao = configuracao.GetSection(SecaoSimulacao).Get<ConfiguracaoSimulacao>() ?? new ConfiguracaoSimulacao();

        var servicos = new ServiceCollection();
        servicos.AddApplication(simulacao);
        servicos.AddInfrastructure(conexao);
        RegistrarApresentacao(servicos);

        return servicos.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
    }

    private static void RegistrarApresentacao(IServiceCollection servicos)
    {
        servicos.AddSingleton<MainWindow>();
        servicos.AddSingleton<INavegador, NavegadorWpf>();
        servicos.AddSingleton<IDialogoService, DialogoServiceWpf>();

        servicos.AddTransient<InicioController>();
        servicos.AddTransient<AutenticacaoController>();
        servicos.AddTransient<HomeController>();
        servicos.AddTransient<RestaurantesController>();
        servicos.AddTransient<CardapioController>();
        servicos.AddTransient<CarrinhoController>();
        servicos.AddTransient<FinalizacaoController>();
        servicos.AddTransient<PedidoController>();
        servicos.AddTransient<HistoricoController>();
        servicos.AddTransient<AvaliacaoController>();
        servicos.AddTransient<EnderecoController>();
        servicos.AddTransient<CartaoController>();
        servicos.AddTransient<PerfilController>();

        servicos.AddTransient<InicioView>();
        servicos.AddTransient<LoginView>();
        servicos.AddTransient<CadastroView>();
        servicos.AddTransient<HomeView>();
        servicos.AddTransient<RestaurantesView>();
        servicos.AddTransient<CardapioView>();
        servicos.AddTransient<CarrinhoView>();
        servicos.AddTransient<FinalizacaoView>();
        servicos.AddTransient<AcompanhamentoView>();
        servicos.AddTransient<HistoricoView>();
        servicos.AddTransient<AvaliacaoView>();
        servicos.AddTransient<EnderecosView>();
        servicos.AddTransient<CartoesView>();
        servicos.AddTransient<PerfilView>();
    }
}
