namespace RotaDoSabor.Infrastructure;

/// <summary>String de conexão lida do appsettings.json (chave ConnectionStrings:RotaDoSabor).</summary>
public sealed record ConfiguracaoBanco(string ConnectionString);
