namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface ICartaoRepository
{
    Task<IReadOnlyList<CartaoSimulado>> ListarAtivosPorClienteAsync(int clienteId);
    Task<CartaoSimulado?> ObterPorIdAsync(int id);
    Task AdicionarAsync(CartaoSimulado cartao);
    Task AtualizarAsync(CartaoSimulado cartao);
    Task RemoverAsync(CartaoSimulado cartao);
    Task<bool> PossuiPagamentosAsync(int cartaoId);
}
