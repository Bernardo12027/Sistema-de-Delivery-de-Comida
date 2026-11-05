using System.Security.Cryptography;

namespace RotaDoSabor.Services;

/// <summary>
/// Hash de senha com PBKDF2-SHA512 + salt aleatório por usuário.
/// Formato salvo no banco: PBKDF2-SHA512$iterações$salt(base64)$hash(base64)
/// (guardar as iterações junto permite aumentá-las no futuro sem invalidar senhas antigas).
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const string Prefixo = "PBKDF2-SHA512";
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 64;
    private const int Iteracoes = 210_000;

    public string Hash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA512, TamanhoHash);

        return $"{Prefixo}${Iteracoes}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string hashArmazenado)
    {
        var partes = hashArmazenado.Split('$');
        if (partes.Length != 4 || partes[0] != Prefixo || !int.TryParse(partes[1], out var iteracoes) || iteracoes <= 0)
            return false;

        try
        {
            var salt = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            if (esperado.Length == 0)
                return false;

            var calculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA512, esperado.Length);

            // Comparação em tempo constante (evita ataques de temporização).
            return CryptographicOperations.FixedTimeEquals(calculado, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
