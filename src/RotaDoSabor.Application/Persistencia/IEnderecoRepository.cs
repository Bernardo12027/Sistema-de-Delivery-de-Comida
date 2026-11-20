namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IEnderecoRepository
{
    Task<IReadOnlyList<Endereco>> ListarAtivosPorClienteAsync(int clienteId);
    Task<Endereco?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Endereco endereco);
    Task AtualizarAsync(IEnumerable<Endereco> enderecos);
    Task RemoverAsync(Endereco endereco);
    Task<bool> PossuiPedidosAsync(int enderecoId);
}
