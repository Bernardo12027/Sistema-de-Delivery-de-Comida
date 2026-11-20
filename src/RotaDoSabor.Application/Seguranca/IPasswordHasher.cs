namespace RotaDoSabor.Application.Seguranca;

/// <summary>Gera e verifica hash de senha. A implementação (PBKDF2) fica na Infraestrutura.</summary>
public interface IPasswordHasher
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hashArmazenado);
}
