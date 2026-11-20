namespace RotaDoSabor.Application.Simulacao;

using System.Diagnostics;
using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

public interface ISimuladorStatusPedido
{
    /// <summary>Avança os pedidos cujo tempo no status atual já passou. Retorna quantos avançaram.</summary>
    Task<int> ProcessarPedidosAsync();

    /// <summary>Avança um pedido imediatamente (botão de demonstração — permite testar sem esperar).</summary>
    Task<StatusPedido> AvancarManualmenteAsync(int clienteId, int pedidoId);

    /// <summary>Laço em segundo plano que chama <see cref="ProcessarPedidosAsync"/> periodicamente.</summary>
    Task ExecutarEmSegundoPlanoAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Simula PENDENTE → EM_PREPARO → SAIU_PARA_ENTREGA → ENTREGUE.
/// Pedidos CANCELADOS nunca são carregados nem avançados. O estado é persistido no banco a cada etapa.
/// </summary>
public sealed class SimuladorStatusPedidoService : ISimuladorStatusPedido
{
    private readonly IPedidoRepository _pedidos;
    private readonly ConfiguracaoSimulacao _configuracao;
    private readonly IRelogio _relogio;

    public SimuladorStatusPedidoService(IPedidoRepository pedidos, ConfiguracaoSimulacao configuracao, IRelogio relogio)
    {
        _pedidos = pedidos;
        _configuracao = configuracao;
        _relogio = relogio;
    }

    public async Task<int> ProcessarPedidosAsync()
    {
        var agora = _relogio.Agora;
        var avancados = 0;

        foreach (var pedido in await _pedidos.ListarEmAndamentoAsync())
        {
            var tempoNoStatus = agora - pedido.DataAtualizacaoStatus;
            if (!pedido.EstaEmAndamento() || tempoNoStatus < _configuracao.ObterDuracao(pedido.Status))
                continue;

            pedido.AvancarStatus(agora);
            if (await _pedidos.AtualizarAsync(pedido)) // false = pedido foi cancelado/alterado no meio tempo
                avancados++;
        }

        return avancados;
    }

    public async Task<StatusPedido> AvancarManualmenteAsync(int clienteId, int pedidoId)
    {
        var pedido = await _pedidos.ObterPorIdAsync(pedidoId);
        if (pedido is null || pedido.ClienteId != clienteId)
            throw new NaoEncontradoException("Pedido");

        if (!pedido.EstaEmAndamento())
            throw new RegraDeNegocioException($"Pedidos {pedido.Status.ParaTexto()} não avançam de status.");

        pedido.AvancarStatus(_relogio.Agora);
        if (!await _pedidos.AtualizarAsync(pedido))
            throw new RegraDeNegocioException("O pedido foi alterado ao mesmo tempo por outro processo. Atualize a tela.");

        return pedido.Status;
    }

    public async Task ExecutarEmSegundoPlanoAsync(CancellationToken cancellationToken)
    {
        if (!_configuracao.Ativa)
            return;

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(1, _configuracao.IntervaloVerificacaoSegundos)));
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await ProcessarComSegurancaAsync();
        }
        catch (OperationCanceledException)
        {
            // Encerramento normal do aplicativo.
        }
    }

    private async Task ProcessarComSegurancaAsync()
    {
        try
        {
            await ProcessarPedidosAsync();
        }
        catch (Exception ex)
        {
            // Uma falha momentânea (ex.: banco indisponível) não pode derrubar a simulação nem o aplicativo.
            Trace.TraceWarning($"[Simulação] Falha ao atualizar pedidos: {ex.Message}");
        }
    }
}
