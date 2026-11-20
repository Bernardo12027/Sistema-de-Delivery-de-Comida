namespace RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Validações reutilizadas pelos construtores das entidades (evita duplicação de "if").
/// </summary>
internal static class Guarda
{
    public static string TextoObrigatorio(string? valor, string campo, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new RegraDeNegocioException($"O campo {campo} é obrigatório.");

        var texto = valor.Trim();
        if (texto.Length > tamanhoMaximo)
            throw new RegraDeNegocioException($"O campo {campo} aceita no máximo {tamanhoMaximo} caracteres.");

        return texto;
    }

    public static string? TextoOpcional(string? valor, string campo, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        var texto = valor.Trim();
        if (texto.Length > tamanhoMaximo)
            throw new RegraDeNegocioException($"O campo {campo} aceita no máximo {tamanhoMaximo} caracteres.");

        return texto;
    }

    public static decimal ValorPositivo(decimal valor, string campo)
    {
        if (valor <= 0)
            throw new RegraDeNegocioException($"O campo {campo} deve ser maior que zero.");
        return valor;
    }

    public static int IdValido(int id, string campo)
    {
        if (id <= 0)
            throw new RegraDeNegocioException($"{campo} inválido.");
        return id;
    }
}
