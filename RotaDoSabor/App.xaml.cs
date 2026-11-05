using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Controllers;
using RotaDoSabor.Data;
using RotaDoSabor.Navegacao;
using RotaDoSabor.Repositories;
using RotaDoSabor.Services;
using RotaDoSabor.Views;

namespace RotaDoSabor;

public partial class App : Application
{
    private ServiceProvider? _servicos;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += AoOcorrerErroNaoTratado;

        try
        {
            _servicos = ConfigurarServicos();

            _servicos.GetRequiredService<DbInitializer>().Inicializar();

            _servicos.GetRequiredService<MainWindow>().Show();
            _servicos.GetRequiredService<AuthController>().MostrarInicial();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível iniciar o aplicativo:\n\n{ex.Message}",
                "Rota do Sabor", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _servicos?.Dispose();
        base.OnExit(e);
    }

    /// <summary>
    /// Composition Root (SOLID - D): único lugar que conhece as classes concretas
    /// e liga cada interface à sua implementação.
    /// </summary>
    private static ServiceProvider ConfigurarServicos()
    {
        var servicos = new ServiceCollection();

        // Banco de dados
        servicos.AddDbContextFactory<AppDbContext>(opcoes => opcoes.UseSqlite(ConfiguracaoBanco.StringConexao));
        servicos.AddSingleton<DbInitializer>();

        // Model / regras de negócio
        servicos.AddSingleton<IPasswordHasher, PasswordHasher>();
        servicos.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        servicos.AddSingleton<IAuthService, AuthService>();
        servicos.AddSingleton<ISessaoUsuario, SessaoUsuario>();

        // Controller + navegação
        servicos.AddSingleton<INavegador, Navegador>();
        servicos.AddSingleton<AuthController>();

        // Views
        servicos.AddSingleton<MainWindow>();
        servicos.AddTransient<WelcomeView>();
        servicos.AddTransient<LoginView>();
        servicos.AddTransient<CadastroView>();
        servicos.AddTransient<HomeView>();

        return servicos.BuildServiceProvider();
    }

    private static void AoOcorrerErroNaoTratado(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"Ocorreu um erro inesperado:\n\n{e.Exception.Message}",
            "Rota do Sabor", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }
}
