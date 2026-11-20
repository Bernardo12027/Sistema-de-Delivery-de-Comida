namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Persistencia;

public interface IHistoricoService
{
    /// <summary>Pedidos do cliente autenticado, do mais recente para o mais antigo (consulta SQL manual).</summary>
    Task<IReadOnlyList<HistoricoPedidoDto>> ListarAsync(int clienteId);
}

public sealed class HistoricoService : IHistoricoService
{
    private readonly IHistoricoPedidoConsulta _consulta;

    public HistoricoService(IHistoricoPedidoConsulta consulta)
    {
        _consulta = consulta;
    }

    public Task<IReadOnlyList<HistoricoPedidoDto>> ListarAsync(int clienteId) => _consulta.ListarPorClienteAsync(clienteId);
}
