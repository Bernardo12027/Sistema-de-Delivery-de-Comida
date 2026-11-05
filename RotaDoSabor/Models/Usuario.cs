namespace RotaDoSabor.Models;

/// <summary>
/// Entidade de usuário (tabela "Usuarios"). A senha NUNCA é guardada em texto puro:
/// apenas o hash PBKDF2 (ver <see cref="Services.PasswordHasher"/>).
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string NomeUsuario { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
