namespace RotaDoSabor.Application.Comum;

using System.Globalization;

/// <summary>Formatação de valores para exibição (moeda brasileira, datas e estrelas).</summary>
public static class Formatacao
{
    public static readonly CultureInfo CulturaBrasil = CultureInfo.GetCultureInfo("pt-BR");

    public static string Moeda(decimal valor) => valor.ToString("C", CulturaBrasil);

    public static string DataHora(DateTime data) => data.ToString("dd/MM/yyyy HH:mm", CulturaBrasil);

    public static string Hora(DateTime data) => data.ToString("HH:mm", CulturaBrasil);

    public static string Estrelas(decimal media, int quantidade)
    {
        if (quantidade == 0)
            return "Sem avaliações";

        var cheias = (int)Math.Round(media, MidpointRounding.AwayFromZero);
        cheias = Math.Clamp(cheias, 0, 5);
        return $"{new string('★', cheias)}{new string('☆', 5 - cheias)} {media.ToString("0.0", CulturaBrasil)} ({quantidade})";
    }
}
