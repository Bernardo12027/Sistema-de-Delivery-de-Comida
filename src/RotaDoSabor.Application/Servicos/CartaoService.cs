namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Validacoes;
using RotaDoSabor.Domain.Entidades;

/// <summary>NOVA FUNCIONALIDADE: gerenciar cartões fictícios.</summary>
public interface ICartaoService
{
    Task<IReadOnlyList<CartaoDto>> ListarAsync(int clienteId);
    Task<int> AdicionarAsync(int clienteId, NovoCartaoDto dados);
    Task<string> RemoverAsync(int clienteId, int cartaoId);
}

public sealed class CartaoService : ICartaoService
{
    private readonly ICartaoRepository _cartoes;
    private readonly IRelogio _relogio;

    public CartaoService(ICartaoRepository cartoes, IRelogio relogio)
    {
        _cartoes = cartoes;
        _relogio = relogio;
    }

    public async Task<IReadOnlyList<CartaoDto>> ListarAsync(int clienteId)
    {
        var cartoes = await _cartoes.ListarAtivosPorClienteAsync(clienteId);
        return cartoes.Select(cartao => cartao.ParaDto()).ToList();
    }

    public async Task<int> AdicionarAsync(int clienteId, NovoCartaoDto dados)
    {
        ValidacaoException.LancarSeHouverErros(ValidadorCartao.Validar(dados, _relogio.Agora));

        // Somente bandeira, últimos 4 dígitos, tipo e apelido seguem adiante. Nome, validade e CVV são descartados.
        var cartao = CartaoSimulado.Criar(clienteId, dados.Numero, dados.Tipo, dados.Apelido, _relogio.Agora);
        await _cartoes.AdicionarAsync(cartao);
        return cartao.Id;
    }

    public async Task<string> RemoverAsync(int clienteId, int cartaoId)
    {
        var cartao = await _cartoes.ObterPorIdAsync(cartaoId);
        if (cartao is null || cartao.ClienteId != clienteId || !cartao.Ativo)
            throw new NaoEncontradoException("Cartão");

        if (await _cartoes.PossuiPagamentosAsync(cartaoId))
        {
            cartao.Inativar();
            await _cartoes.AtualizarAsync(cartao);
            return "Cartão removido. Os pagamentos antigos continuam registrados no histórico.";
        }

        await _cartoes.RemoverAsync(cartao);
        return "Cartão removido.";
    }
}
