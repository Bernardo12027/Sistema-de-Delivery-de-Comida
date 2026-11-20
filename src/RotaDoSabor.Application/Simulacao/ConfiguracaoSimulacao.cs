namespace RotaDoSabor.Application.Simulacao;

using RotaDoSabor.Domain.Enums;

/// <summary>
/// Intervalos da simulação de acompanhamento (lidos do appsettings.json, seção "SimulacaoPedido").
/// Os valores padrão são curtos para facilitar a apresentação.
/// </summary>
public sealed class ConfiguracaoSimulacao
{
    public bool Ativa { get; set; } = true;
    public int IntervaloVerificacaoSegundos { get; set; } = 5;
    public int SegundosEmPendente { get; set; } = 20;
    public int SegundosEmPreparo { get; set; } = 30;
    public int SegundosSaiuParaEntrega { get; set; } = 30;

    /// <summary>Tempo que o pedido permanece no status informado antes de avançar.</summary>
    public TimeSpan ObterDuracao(StatusPedido status) => status switch
    {
        StatusPedido.Pendente => TimeSpan.FromSeconds(SegundosEmPendente),
        StatusPedido.EmPreparo => TimeSpan.FromSeconds(SegundosEmPreparo),
        StatusPedido.SaiuParaEntrega => TimeSpan.FromSeconds(SegundosSaiuParaEntrega),
        _ => TimeSpan.MaxValue
    };
}
