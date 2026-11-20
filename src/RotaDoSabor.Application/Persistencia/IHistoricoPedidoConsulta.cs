namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Application.Dtos;

/// <summary>
/// Consulta do histórico de pedidos. Implementada com SQL MANUAL (ADO.NET) em
/// RotaDoSabor.Infrastructure/Repositorios/Sql/HistoricoPedidoSqlRepository.cs.
/// </summary>
public interface IHistoricoPedidoConsulta
{
    Task<IReadOnlyList<HistoricoPedidoDto>> ListarPorClienteAsync(int clienteId);
}
