namespace RotaDoSabor.Application.Dtos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Domain.Enums;

public sealed record CupomAplicadoDto(string Codigo, string Descricao, decimal Percentual, decimal Desconto, decimal TotalComDesconto)
{
    public string Resumo => $"Cupom {Codigo} ({Percentual:0.##}%): -{Formatacao.Moeda(Desconto)}";
}

public sealed record FinalizarPedidoDto(int EnderecoId, FormaPagamento FormaPagamento, int? CartaoId, string? CodigoCupom);

public sealed record PedidoFinalizadoDto(int PedidoId, StatusPagamento StatusPagamento, string Mensagem)
{
    public bool PagamentoRecusado => StatusPagamento == StatusPagamento.Recusado;
}

public sealed record ItemPedidoDto(int ProdutoId, string NomeProduto, int Quantidade, decimal PrecoUnitario, decimal Subtotal)
{
    public string PrecoUnitarioTexto => Formatacao.Moeda(PrecoUnitario);
    public string SubtotalTexto => Formatacao.Moeda(Subtotal);
}

public sealed record DetalhePedidoDto(
    int Id,
    DateTime DataHora,
    int RestauranteId,
    string NomeRestaurante,
    StatusPedido Status,
    decimal Subtotal,
    decimal Desconto,
    decimal TaxaEntrega,
    decimal ValorTotal,
    string EnderecoEntrega,
    FormaPagamento FormaPagamento,
    StatusPagamento StatusPagamento,
    string? CodigoCupom,
    int TempoEstimadoMinutos,
    DateTime PrevisaoEntrega,
    IReadOnlyList<ItemPedidoDto> Itens,
    bool PodeCancelar,
    bool PodeAvaliar)
{
    public string StatusTexto => Status.ParaTexto();
    public string PagamentoTexto => $"{FormaPagamento.ParaTexto()} — {StatusPagamento.ParaTexto()}";

    /// <summary>Percentual para a barra de progresso (sem animação).</summary>
    public int Progresso => Status switch
    {
        StatusPedido.Pendente => 25,
        StatusPedido.EmPreparo => 50,
        StatusPedido.SaiuParaEntrega => 75,
        StatusPedido.Entregue => 100,
        _ => 0
    };
}

public sealed record RepeticaoPedidoDto(int ItensAdicionados, IReadOnlyList<string> ItensIndisponiveis)
{
    public bool AlgumItemIndisponivel => ItensIndisponiveis.Count > 0;
}

/// <summary>Linha do histórico (preenchida pela consulta SQL manual).</summary>
public sealed record HistoricoPedidoDto(int PedidoId, DateTime DataHora, string NomeRestaurante, decimal ValorTotal, StatusPedido Status, bool JaAvaliado)
{
    public string DataHoraTexto => Formatacao.DataHora(DataHora);
    public string ValorTotalTexto => Formatacao.Moeda(ValorTotal);
    public string StatusTexto => Status.ParaTexto();
    public bool PodeAvaliar => Status == StatusPedido.Entregue && !JaAvaliado;
    public string AvaliacaoTexto => JaAvaliado ? "Avaliado" : PodeAvaliar ? "Pode avaliar" : "-";
}

public sealed record InfoAvaliacaoDto(int PedidoId, string NomeRestaurante, bool PodeAvaliar, string Mensagem);

public sealed record NovaAvaliacaoDto(int PedidoId, int Nota, string? Comentario);
