namespace RotaDoSabor.Application.Validacoes;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Domain.Entidades;

public static class ValidadorCadastroCliente
{
    public const int TamanhoMinimoSenha = 6;

    public static IReadOnlyList<string> Validar(CadastroClienteDto dto)
    {
        var erros = new List<string>();

        if (RegrasValidacao.Vazio(dto.Nome))
            erros.Add("Informe o nome.");
        else if (dto.Nome.Trim().Length > Cliente.TamanhoMaximoNome)
            erros.Add($"O nome aceita no máximo {Cliente.TamanhoMaximoNome} caracteres.");

        if (!RegrasValidacao.EmailValido(dto.Email))
            erros.Add("Informe um e-mail válido.");

        if (!RegrasValidacao.NomeUsuarioValido(dto.NomeUsuario))
            erros.Add("Nome de usuário deve ter de 3 a 30 caracteres (letras, números, ponto ou _).");

        if (!RegrasValidacao.TelefoneValido(dto.Telefone))
            erros.Add("Telefone deve ter DDD + número (10 ou 11 dígitos).");

        erros.AddRange(ValidarSenha(dto.Senha, dto.ConfirmacaoSenha));
        return erros;
    }

    private static IEnumerable<string> ValidarSenha(string senha, string confirmacao)
    {
        if (string.IsNullOrEmpty(senha))
        {
            yield return "Informe a senha.";
            yield break;
        }

        if (senha.Length < TamanhoMinimoSenha || !senha.Any(char.IsLetter) || !senha.Any(char.IsDigit))
            yield return $"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres, com letras e números.";

        if (senha != confirmacao)
            yield return "A confirmação de senha não confere.";
    }
}
