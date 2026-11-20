namespace RotaDoSabor.Application.Validacoes;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Domain.Entidades;

public static class ValidadorPerfil
{
    public static IReadOnlyList<string> Validar(AtualizarPerfilDto dto)
    {
        var erros = new List<string>();

        if (RegrasValidacao.Vazio(dto.Nome))
            erros.Add("Informe o nome.");
        else if (dto.Nome.Trim().Length > Cliente.TamanhoMaximoNome)
            erros.Add($"O nome aceita no máximo {Cliente.TamanhoMaximoNome} caracteres.");

        if (!RegrasValidacao.TelefoneValido(dto.Telefone))
            erros.Add("Telefone deve ter DDD + número (10 ou 11 dígitos).");

        return erros;
    }
}
