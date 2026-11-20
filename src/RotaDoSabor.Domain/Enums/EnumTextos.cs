namespace RotaDoSabor.Domain.Enums;

/// <summary>
/// Textos de exibição dos enums, no formato usado nos requisitos (ex.: EM_PREPARO).
/// </summary>
public static class EnumTextos
{
    public static string ParaTexto(this StatusPedido status) => status switch
    {
        StatusPedido.Pendente => "PENDENTE",
        StatusPedido.EmPreparo => "EM_PREPARO",
        StatusPedido.SaiuParaEntrega => "SAIU_PARA_ENTREGA",
        StatusPedido.Entregue => "ENTREGUE",
        StatusPedido.Cancelado => "CANCELADO",
        _ => status.ToString()
    };

    public static string ParaTexto(this StatusPagamento status) => status switch
    {
        StatusPagamento.Pendente => "PENDENTE",
        StatusPagamento.Aprovado => "APROVADO",
        StatusPagamento.Recusado => "RECUSADO",
        StatusPagamento.Estornado => "ESTORNADO",
        _ => status.ToString()
    };

    public static string ParaTexto(this FormaPagamento forma) => forma switch
    {
        FormaPagamento.Pix => "Pix",
        FormaPagamento.CartaoCredito => "Cartão de crédito",
        FormaPagamento.CartaoDebito => "Cartão de débito",
        FormaPagamento.Dinheiro => "Dinheiro",
        _ => forma.ToString()
    };

    public static string ParaTexto(this TipoCartao tipo) => tipo switch
    {
        TipoCartao.Credito => "Crédito",
        TipoCartao.Debito => "Débito",
        _ => tipo.ToString()
    };
}
