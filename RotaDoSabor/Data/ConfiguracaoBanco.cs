using System.IO;

namespace RotaDoSabor.Data;

/// <summary>
/// Onde fica o arquivo do banco SQLite:
/// %LocalAppData%\RotaDoSabor\rotadosabor.db
/// (pasta estável: não é apagada quando o projeto é recompilado).
/// </summary>
public static class ConfiguracaoBanco
{
    public static string CaminhoArquivo { get; } = CriarCaminho();

    public static string StringConexao => $"Data Source={CaminhoArquivo}";

    private static string CriarCaminho()
    {
        var pasta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RotaDoSabor");

        Directory.CreateDirectory(pasta);
        return Path.Combine(pasta, "rotadosabor.db");
    }
}
