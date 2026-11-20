namespace RotaDoSabor.Application.Pagamentos;

/// <summary>Gera códigos de transação fictícios (não há integração financeira real).</summary>
internal static class CodigoTransacaoSimulada
{
    public static string Gerar(string prefixo) => $"{prefixo}-{Guid.NewGuid():N}"[..(prefixo.Length + 13)].ToUpperInvariant();
}
