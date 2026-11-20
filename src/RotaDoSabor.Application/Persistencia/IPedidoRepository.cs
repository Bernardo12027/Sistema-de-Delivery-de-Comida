namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IPedidoRepository
{
    /// <summary>Grava o pedido com seus itens e pagamento em uma única transação.</summary>
    Task AdicionarAsync(Pedido pedido);

    /// <summary>Pedido com itens, pagamento e avaliação (para regras de negócio).</summary>
    Task<Pedido?> ObterPorIdAsync(int id);

    /// <summary>Pedido completo para exibição (inclui restaurante, endereço e cupom).</summary>
    Task<Pedido?> ObterDetalheAsync(int id);

    /// <summary>Pedidos PENDENTE, EM_PREPARO ou SAIU_PARA_ENTREGA (usados pela simulação).</summary>
    Task<IReadOnlyList<Pedido>> ListarEmAndamentoAsync();

    /// <summary>Atualiza status/pagamento. Retorna false se o pedido foi alterado por outro processo (concorrência).</summary>
    Task<bool> AtualizarAsync(Pedido pedido);
}
