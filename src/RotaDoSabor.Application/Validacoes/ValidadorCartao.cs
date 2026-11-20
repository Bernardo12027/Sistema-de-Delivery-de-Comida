namespace RotaDoSabor.Application.Validacoes;

using System.Globalization;
using RotaDoSabor.Application.Dtos;

/// <summary>Valida o formulário do cartão fictício. Os dados sensíveis só passam por aqui e não são guardados.</summary>
public static class ValidadorCartao
{
    public static IReadOnlyList<string> Validar(NovoCartaoDto dto, DateTime agora)
    {
        var erros = new List<string>();

        var digitos = RegrasValidacao.ApenasDigitos(dto.Numero);
        if (digitos.Length is < 13 or > 19)
            erros.Add("Número do cartão deve ter entre 13 e 19 dígitos.");

        if (RegrasValidacao.Vazio(dto.NomeTitular))
            erros.Add("Informe o nome impresso no cartão.");

        if (!ValidadeEhFutura(dto.Validade, agora))
            erros.Add("Validade deve estar no formato MM/AA e não pode estar vencida.");

        var cvv = RegrasValidacao.ApenasDigitos(dto.Cvv);
        if (cvv.Length is < 3 or > 4 || cvv.Length != (dto.Cvv ?? string.Empty).Trim().Length)
            erros.Add("CVV deve ter 3 ou 4 dígitos (ele não será armazenado).");

        return erros;
    }

    private static bool ValidadeEhFutura(string? validade, DateTime agora)
    {
        if (!DateTime.TryParseExact(validade?.Trim(), "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
            return false;

        var ultimoDiaDoMes = new DateTime(data.Year, data.Month, DateTime.DaysInMonth(data.Year, data.Month));
        return ultimoDiaDoMes >= agora.Date;
    }
}
