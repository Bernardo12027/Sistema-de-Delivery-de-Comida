namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(int id);
    Task<Cliente?> ObterPorEmailAsync(string email);
    Task<Cliente?> ObterPorNomeUsuarioAsync(string nomeUsuario);
    Task<bool> ExisteEmailAsync(string email);
    Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario);
    Task AdicionarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
}
