using System.Text.RegularExpressions;
using RotaDoSabor.Models;

namespace RotaDoSabor.Services;

/// <summary>
/// Regras de validação do cadastro (SOLID - S: só valida, não acessa banco nem tela).
/// Retorna a primeira mensagem de erro encontrada, ou null se tudo estiver válido.
/// </summary>
public static class ValidadorCadastro
{
    public const int SenhaTamanhoMinimo = 6;
    public const int SenhaTamanhoMaximo = 64;

    // '@' é proibido no nome de usuário: o login aceita usuário OU e-mail,
    // então um nome parecido com e-mail geraria ambiguidade (e permitiria "roubar" o e-mail de outra pessoa).
    private static readonly Regex PadraoNomeUsuario = new(@"^[\p{L}\p{N}_.\-]{3,30}$", RegexOptions.Compiled);

    private static readonly Regex PadraoEmail = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public static string? Validar(DadosCadastro dados)
    {
        if (!PadraoNomeUsuario.IsMatch(dados.NomeUsuario))
            return "O usuário deve ter de 3 a 30 caracteres, usando apenas letras, números, _ . ou -";

        if (dados.Email.Length > 120 || !PadraoEmail.IsMatch(dados.Email))
            return "Informe um e-mail válido.";

        if (dados.Senha.Length < SenhaTamanhoMinimo || dados.Senha.Length > SenhaTamanhoMaximo)
            return $"A senha deve ter de {SenhaTamanhoMinimo} a {SenhaTamanhoMaximo} caracteres.";

        if (dados.Senha != dados.ConfirmacaoSenha)
            return "As senhas não coincidem.";

        return null;
    }
}
