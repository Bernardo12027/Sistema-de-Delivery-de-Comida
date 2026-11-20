namespace RotaDoSabor.Infrastructure.Seguranca;

using System.Security.Cryptography;
using RotaDoSabor.Application.Seguranca;

/// <summary>
/// Hash de senha com PBKDF2-SHA256, salt aleatório de 16 bytes e 100.000 iterações.
/// Formato gravado: PBKDF2-SHA256$iteracoes$saltBase64$hashBase64 (a senha original não é recuperável).
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string Algoritmo = "PBKDF2-SHA256";
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 100_000;
    private const char Separador = '$';

    public string GerarHash(string senha)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
        return string.Join(Separador, Algoritmo, Iteracoes, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public bool Verificar(string senha, string hashArmazenado)
    {
        var partes = hashArmazenado.Split(Separador);
        if (partes.Length != 4 || partes[0] != Algoritmo || !int.TryParse(partes[1], out var iteracoes))
            return false;

        try
        {
            var salt = Convert.FromBase64String(partes[2]);
            var hashEsperado = Convert.FromBase64String(partes[3]);
            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, hashEsperado.Length);
            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado); // comparação em tempo constante
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
