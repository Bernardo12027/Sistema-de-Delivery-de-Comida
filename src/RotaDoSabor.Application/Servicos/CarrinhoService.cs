namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Domain.Excecoes;

/// <summary>Operações sobre o carrinho da sessão. O preço e a disponibilidade vêm sempre do banco.</summary>
public interface ICarrinhoService
{
    CarrinhoDto Obter();

    /// <summary>
    /// Adiciona 1 unidade. Se o carrinho tiver itens de outro restaurante e <paramref name="substituirCarrinhoAtual"/>
    /// for false, nada é alterado e é pedido que a tela confirme a troca.
    /// </summary>
    Task<ResultadoAdicaoCarrinho> AdicionarAsync(int produtoId, bool substituirCarrinhoAtual);

    void Aumentar(int produtoId);
    void Diminuir(int produtoId);
    void Remover(int produtoId);
    void Limpar();
}

public sealed class CarrinhoService : ICarrinhoService
{
    private readonly ISessaoUsuario _sessao;
    private readonly IProdutoRepository _produtos;

    public CarrinhoService(ISessaoUsuario sessao, IProdutoRepository produtos)
    {
        _sessao = sessao;
        _produtos = produtos;
    }

    public CarrinhoDto Obter() => _sessao.Carrinho.ParaDto();

    public async Task<ResultadoAdicaoCarrinho> AdicionarAsync(int produtoId, bool substituirCarrinhoAtual)
    {
        var produto = await _produtos.ObterPorIdAsync(produtoId) ?? throw new NaoEncontradoException("Produto");
        if (!produto.Disponivel)
            throw new RegraDeNegocioException($"\"{produto.Nome}\" está indisponível no momento.");

        var carrinho = _sessao.Carrinho;
        if (carrinho.PertenceAOutroRestaurante(produto.RestauranteId))
        {
            if (!substituirCarrinhoAtual)
                return ResultadoAdicaoCarrinho.ConfirmacaoTrocaRestauranteNecessaria;
            carrinho.Limpar();
        }

        carrinho.Adicionar(produto, produto.Restaurante?.Nome ?? string.Empty);
        return ResultadoAdicaoCarrinho.Adicionado;
    }

    public void Aumentar(int produtoId) => _sessao.Carrinho.Aumentar(produtoId);

    public void Diminuir(int produtoId) => _sessao.Carrinho.Diminuir(produtoId);

    public void Remover(int produtoId) => _sessao.Carrinho.Remover(produtoId);

    public void Limpar() => _sessao.Carrinho.Limpar();
}
