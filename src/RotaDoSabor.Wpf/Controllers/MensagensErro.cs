namespace RotaDoSabor.Wpf.Controllers;

/// <summary>Converte exceções técnicas em mensagens compreensíveis para o usuário.</summary>
public static class MensagensErro
{
    public static string Traduzir(Exception ex)
    {
        if (EhFalhaDeConexao(ex))
            return "Não foi possível conectar ao banco de dados (SQL Server LocalDB).\n\n" +
                   "Verifique se o LocalDB está instalado (Visual Studio Installer → \"Armazenamento e processamento de dados\") " +
                   "e se a string de conexão do appsettings.json está correta.\n\nDetalhe: " + ex.GetBaseException().Message;

        if (ContemTipo(ex, "DbUpdateException"))
            return "Não foi possível gravar os dados no banco.\n\nDetalhe: " + ex.GetBaseException().Message;

        return "Ocorreu um erro inesperado.\n\nDetalhe: " + ex.GetBaseException().Message;
    }

    public static bool EhFalhaDeConexao(Exception ex) =>
        ContemTipo(ex, "SqlException") || ContemTipo(ex, "RetryLimitExceededException") || ContemTipo(ex, "Win32Exception");

    private static bool ContemTipo(Exception? ex, string nomeTipo)
    {
        for (var atual = ex; atual is not null; atual = atual.InnerException)
        {
            if (atual.GetType().Name == nomeTipo)
                return true;
        }
        return false;
    }
}
