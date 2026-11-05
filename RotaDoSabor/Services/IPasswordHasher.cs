namespace RotaDoSabor.Services;

/// <summary>
/// SOLID (I e O): interface mínima. Para trocar o algoritmo (ex.: Argon2/BCrypt)
/// basta criar outra implementação, sem alterar o AuthService.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string senha);

    bool Verificar(string senha, string hashArmazenado);
}
