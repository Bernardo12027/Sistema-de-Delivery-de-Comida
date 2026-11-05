using RotaDoSabor.Common;
using RotaDoSabor.Models;

namespace RotaDoSabor.Services;

public interface IAuthService
{
    /// <param name="identificador">Nome de usuário ou e-mail.</param>
    Task<Resultado<Usuario>> EntrarAsync(string identificador, string senha);

    Task<Resultado> CadastrarAsync(DadosCadastro dados);
}
