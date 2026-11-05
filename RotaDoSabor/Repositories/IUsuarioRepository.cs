using RotaDoSabor.Models;

namespace RotaDoSabor.Repositories;

/// <summary>
/// Contrato de acesso a dados de usuários.
/// SOLID (D e L): os serviços dependem desta abstração, não do EF; qualquer implementação
/// (ex.: em memória, para testes) pode substituir a real.
/// </summary>
public interface IUsuarioRepository
{
    /// <summary>Busca por nome de usuário OU e-mail (sem diferenciar maiúsculas/minúsculas).</summary>
    Task<Usuario?> ObterPorNomeOuEmailAsync(string identificador);

    Task<bool> ExisteNomeUsuarioAsync(string nomeUsuario);

    Task<bool> ExisteEmailAsync(string email);

    Task AdicionarAsync(Usuario usuario);
}
