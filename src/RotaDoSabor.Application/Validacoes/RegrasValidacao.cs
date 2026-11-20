namespace RotaDoSabor.Application.Validacoes;

using System.Text.RegularExpressions;

/// <summary>Regras simples reaproveitadas pelos validadores de formulário.</summary>
public static partial class RegrasValidacao
{
    public static bool Vazio(string? valor) => string.IsNullOrWhiteSpace(valor);

    public static string ApenasDigitos(string? valor) => new((valor ?? string.Empty).Where(char.IsDigit).ToArray());

    public static bool EmailValido(string? email) => !Vazio(email) && RegexEmail().IsMatch(email!.Trim());

    public static bool NomeUsuarioValido(string? nomeUsuario) => !Vazio(nomeUsuario) && RegexNomeUsuario().IsMatch(nomeUsuario!.Trim());

    /// <summary>Telefone é opcional; se informado, precisa ter 10 ou 11 dígitos (DDD + número).</summary>
    public static bool TelefoneValido(string? telefone)
    {
        if (Vazio(telefone))
            return true;
        var digitos = ApenasDigitos(telefone);
        return digitos.Length is 10 or 11;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex RegexEmail();

    [GeneratedRegex(@"^[a-zA-Z0-9._]{3,30}$")]
    private static partial Regex RegexNomeUsuario();
}
