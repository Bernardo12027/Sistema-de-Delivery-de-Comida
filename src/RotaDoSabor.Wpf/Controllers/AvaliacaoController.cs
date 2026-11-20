namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

public sealed class AvaliacaoController : ControllerBase
{
    private readonly IAvaliacaoService _avaliacoes;
    private readonly ISessaoUsuario _sessao;

    public AvaliacaoController(IAvaliacaoService avaliacoes, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _avaliacoes = avaliacoes;
        _sessao = sessao;
    }

    public Task<InfoAvaliacaoDto?> CarregarAsync(int pedidoId) =>
        ExecutarAsync(() => _avaliacoes.ObterInfoAsync(_sessao.ObterClienteIdObrigatorio(), pedidoId));

    public async Task EnviarAsync(int pedidoId, int? nota, string comentario)
    {
        if (nota is null)
        {
            Dialogo.Avisar("Escolha de 1 a 5 estrelas.");
            return;
        }

        var sucesso = await ExecutarAsync(() =>
            _avaliacoes.AvaliarAsync(_sessao.ObterClienteIdObrigatorio(), new NovaAvaliacaoDto(pedidoId, nota.Value, comentario)));

        if (!sucesso)
            return;

        Dialogo.Informar("Obrigado pela avaliação! A média do restaurante foi atualizada.");
        Navegador.Navegar(Tela.Historico);
    }
}
