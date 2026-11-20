namespace RotaDoSabor.Wpf;

using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Application.Simulacao;
using RotaDoSabor.Infrastructure.Persistencia;
using RotaDoSabor.Wpf.Configuracao;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>
/// Ponto de entrada: monta a injeção de dependência, cria/atualiza o banco (migrations + seed),
/// inicia a simulação de status em segundo plano e abre a primeira tela.
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly CancellationTokenSource _encerramento = new();
    private ServiceProvider? _servicos;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ConfigurarCulturaBrasileira();

        try
        {
            _servicos = CompositionRoot.CriarServicos();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Falha ao carregar a configuração (appsettings.json).\n\n" + ex.Message, "Rota do Sabor",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        var janela = _servicos.GetRequiredService<MainWindow>();
        janela.Show();

        if (!await InicializarBancoAsync())
        {
            Shutdown(1);
            return;
        }

        var simulador = _servicos.GetRequiredService<ISimuladorStatusPedido>();
        _ = Task.Run(() => simulador.ExecutarEmSegundoPlanoAsync(_encerramento.Token));

        _servicos.GetRequiredService<INavegador>().Navegar(Tela.Inicio);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _encerramento.Cancel();
        _servicos?.Dispose();
        base.OnExit(e);
    }

    private async Task<bool> InicializarBancoAsync()
    {
        try
        {
            await _servicos!.GetRequiredService<IInicializadorBanco>().InicializarAsync(_encerramento.Token);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(MensagensErro.Traduzir(ex) +
                "\n\nO aplicativo será fechado. Consulte o README (seção \"Banco de dados\").",
                "Rota do Sabor — banco de dados", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private static void ConfigurarCulturaBrasileira()
    {
        var cultura = CultureInfo.GetCultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = cultura;
        CultureInfo.DefaultThreadCurrentUICulture = cultura;
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(cultura.IetfLanguageTag)));
    }
}
