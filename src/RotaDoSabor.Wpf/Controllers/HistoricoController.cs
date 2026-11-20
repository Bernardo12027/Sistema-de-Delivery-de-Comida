namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Histórico de pedidos (dados vêm da consulta SQL manual).</summary>
public sealed class HistoricoController : ControllerBase
{
    private readonly IHistoricoService _historico;
    private readonly ISessaoUsuario _sessao;

    public HistoricoController(IHistoricoService historico, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _historico = historico;
        _sessao = sessao;
    }

    public async Task<IReadOnlyList<HistoricoPedidoDto>> CarregarAsync() =>
        await ExecutarAsync(() => _historico.ListarAsync(_sessao.ObterClienteIdObrigatorio())) ?? [];

    public void VerDetalhes(int pedidoId) => Navegador.Navegar(Tela.Acompanhamento, pedidoId);

    public void Avaliar(int pedidoId) => Navegador.Navegar(Tela.Avaliacao, pedidoId);
}
