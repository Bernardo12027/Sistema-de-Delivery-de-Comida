namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>NOVA FUNCIONALIDADE: gerenciar cartões fictícios.</summary>
public sealed class CartaoController : ControllerBase
{
    private readonly ICartaoService _cartoes;
    private readonly ISessaoUsuario _sessao;

    public CartaoController(ICartaoService cartoes, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _cartoes = cartoes;
        _sessao = sessao;
    }

    public async Task<IReadOnlyList<CartaoDto>> ListarAsync() =>
        await ExecutarAsync(() => _cartoes.ListarAsync(_sessao.ObterClienteIdObrigatorio())) ?? [];

    public async Task<bool> AdicionarAsync(NovoCartaoDto dados)
    {
        var sucesso = await ExecutarAsync(() => _cartoes.AdicionarAsync(_sessao.ObterClienteIdObrigatorio(), dados));
        if (sucesso)
            Dialogo.Informar("Cartão fictício cadastrado. Somente bandeira e os 4 últimos dígitos foram salvos.");
        return sucesso;
    }

    public async Task<bool> RemoverAsync(CartaoDto cartao)
    {
        if (!Dialogo.Confirmar($"Remover o cartão {cartao.Descricao}?", "Remover cartão"))
            return false;

        var mensagem = await ExecutarAsync(() => _cartoes.RemoverAsync(_sessao.ObterClienteIdObrigatorio(), cartao.Id));
        if (mensagem is not null)
            Dialogo.Informar(mensagem);
        return mensagem is not null;
    }
}
