namespace RotaDoSabor.Infrastructure.Repositorios.Sql;

using System.Data;
using Microsoft.Data.SqlClient;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Enums;

/// <summary>
/// FUNCIONALIDADE COM SQL MANUAL (ADO.NET): Histórico de pedidos do cliente autenticado.
/// - Filtra pelo Id do cliente usando parâmetro (@ClienteId) — nenhum dado do usuário é concatenado na query.
/// - Faz JOIN com Restaurantes para trazer o nome do restaurante.
/// - Ordena do pedido mais recente para o mais antigo.
/// </summary>
public sealed class HistoricoPedidoSqlRepository : IHistoricoPedidoConsulta
{
    private const string ConsultaHistorico = """
        SELECT  p.Id            AS PedidoId,
                p.DataHora      AS DataHora,
                r.Nome          AS NomeRestaurante,
                p.ValorTotal    AS ValorTotal,
                p.Status        AS Status,
                CAST(CASE WHEN a.Id IS NULL THEN 0 ELSE 1 END AS bit) AS JaAvaliado
        FROM    dbo.Pedidos AS p
                INNER JOIN dbo.Restaurantes AS r ON r.Id = p.RestauranteId
                LEFT JOIN  dbo.Avaliacoes   AS a ON a.PedidoId = p.Id
        WHERE   p.ClienteId = @ClienteId
        ORDER BY p.DataHora DESC, p.Id DESC;
        """;

    private readonly string _connectionString;

    public HistoricoPedidoSqlRepository(ConfiguracaoBanco configuracao)
    {
        _connectionString = configuracao.ConnectionString;
    }

    public async Task<IReadOnlyList<HistoricoPedidoDto>> ListarPorClienteAsync(int clienteId)
    {
        await using var conexao = new SqlConnection(_connectionString);
        await using var comando = new SqlCommand(ConsultaHistorico, conexao);
        comando.Parameters.Add(new SqlParameter("@ClienteId", SqlDbType.Int) { Value = clienteId });

        await conexao.OpenAsync();
        await using var leitor = await comando.ExecuteReaderAsync();

        var colunaId = leitor.GetOrdinal("PedidoId");
        var colunaData = leitor.GetOrdinal("DataHora");
        var colunaRestaurante = leitor.GetOrdinal("NomeRestaurante");
        var colunaValor = leitor.GetOrdinal("ValorTotal");
        var colunaStatus = leitor.GetOrdinal("Status");
        var colunaAvaliado = leitor.GetOrdinal("JaAvaliado");

        var historico = new List<HistoricoPedidoDto>();
        while (await leitor.ReadAsync())
        {
            historico.Add(new HistoricoPedidoDto(
                leitor.GetInt32(colunaId),
                leitor.GetDateTime(colunaData),
                leitor.GetString(colunaRestaurante),
                leitor.GetDecimal(colunaValor),
                Enum.Parse<StatusPedido>(leitor.GetString(colunaStatus)),
                leitor.GetBoolean(colunaAvaliado)));
        }

        return historico;
    }
}
