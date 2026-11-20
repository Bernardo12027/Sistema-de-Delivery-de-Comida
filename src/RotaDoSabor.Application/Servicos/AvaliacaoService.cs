namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Enums;
using RotaDoSabor.Domain.Excecoes;

public interface IAvaliacaoService
{
    Task<InfoAvaliacaoDto> ObterInfoAsync(int clienteId, int pedidoId);

    /// <summary>Registra a avaliação (somente pedidos ENTREGUES, uma vez) e atualiza a média do restaurante.</summary>
    Task AvaliarAsync(int clienteId, NovaAvaliacaoDto dados);
}

public sealed class AvaliacaoService : IAvaliacaoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IRestauranteRepository _restaurantes;
    private readonly IAvaliacaoRepository _avaliacoes;
    private readonly IRelogio _relogio;

    public AvaliacaoService(IPedidoRepository pedidos, IRestauranteRepository restaurantes, IAvaliacaoRepository avaliacoes, IRelogio relogio)
    {
        _pedidos = pedidos;
        _restaurantes = restaurantes;
        _avaliacoes = avaliacoes;
        _relogio = relogio;
    }

    public async Task<InfoAvaliacaoDto> ObterInfoAsync(int clienteId, int pedidoId)
    {
        var pedido = await _pedidos.ObterDetalheAsync(pedidoId);
        if (pedido is null || pedido.ClienteId != clienteId)
            throw new NaoEncontradoException("Pedido");

        var nomeRestaurante = pedido.Restaurante?.Nome ?? string.Empty;
        return new InfoAvaliacaoDto(pedido.Id, nomeRestaurante, pedido.PodeSerAvaliado(), MotivoBloqueio(pedido));
    }

    public async Task AvaliarAsync(int clienteId, NovaAvaliacaoDto dados)
    {
        var pedido = await _pedidos.ObterPorIdAsync(dados.PedidoId);
        if (pedido is null || pedido.ClienteId != clienteId)
            throw new NaoEncontradoException("Pedido");

        var motivo = MotivoBloqueio(pedido);
        if (!pedido.PodeSerAvaliado())
            throw new RegraDeNegocioException(motivo);

        var restaurante = await _restaurantes.ObterPorIdAsync(pedido.RestauranteId) ?? throw new NaoEncontradoException("Restaurante");
        var avaliacao = new Avaliacao(pedido, dados.Nota, dados.Comentario, _relogio.Agora);
        restaurante.RegistrarAvaliacao(avaliacao.Nota);

        await _avaliacoes.RegistrarAsync(avaliacao, restaurante);
    }

    private static string MotivoBloqueio(Pedido pedido)
    {
        if (pedido.Avaliacao is not null)
            return "Este pedido já foi avaliado.";
        if (pedido.Status != StatusPedido.Entregue)
            return $"Só é possível avaliar pedidos ENTREGUES (status atual: {pedido.Status.ParaTexto()}).";
        return string.Empty;
    }
}
