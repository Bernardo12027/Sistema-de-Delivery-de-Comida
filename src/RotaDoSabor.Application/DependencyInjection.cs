namespace RotaDoSabor.Application;

using Microsoft.Extensions.DependencyInjection;
using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Pagamentos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Application.Simulacao;

/// <summary>Registro dos serviços da camada de aplicação no contêiner de injeção de dependência.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, ConfiguracaoSimulacao configuracaoSimulacao)
    {
        services.AddSingleton(configuracaoSimulacao);
        services.AddSingleton<IRelogio, RelogioSistema>();
        services.AddSingleton<ISessaoUsuario, SessaoUsuario>();

        // Strategies de pagamento (OCP): uma nova forma = uma nova linha aqui + uma nova classe.
        services.AddSingleton<IPagamentoStrategy, PixPagamentoStrategy>();
        services.AddSingleton<IPagamentoStrategy, CartaoCreditoPagamentoStrategy>();
        services.AddSingleton<IPagamentoStrategy, CartaoDebitoPagamentoStrategy>();
        services.AddSingleton<IPagamentoStrategy, DinheiroPagamentoStrategy>();
        services.AddSingleton<IProcessadorPagamento, ProcessadorPagamentoSimulado>();

        services.AddTransient<IAutenticacaoService, AutenticacaoService>();
        services.AddTransient<ICatalogoService, CatalogoService>();
        services.AddTransient<ICarrinhoService, CarrinhoService>();
        services.AddTransient<ICupomService, CupomService>();
        services.AddTransient<IEnderecoService, EnderecoService>();
        services.AddTransient<ICartaoService, CartaoService>();
        services.AddTransient<IPerfilService, PerfilService>();
        services.AddTransient<ICheckoutService, CheckoutService>();
        services.AddTransient<IPedidoService, PedidoService>();
        services.AddTransient<IHistoricoService, HistoricoService>();
        services.AddTransient<IAvaliacaoService, AvaliacaoService>();
        services.AddSingleton<ISimuladorStatusPedido, SimuladorStatusPedidoService>();

        return services;
    }
}
