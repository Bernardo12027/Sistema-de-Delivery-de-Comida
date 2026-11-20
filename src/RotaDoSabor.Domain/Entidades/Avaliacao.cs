namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Avaliação (1 a 5 estrelas) de um pedido entregue. Cada pedido aceita apenas uma avaliação.</summary>
public class Avaliacao
{
    public const int NotaMinima = 1;
    public const int NotaMaxima = 5;
    public const int TamanhoMaximoComentario = 500;

    public int Id { get; private set; }
    public int Nota { get; private set; }
    public string? Comentario { get; private set; }
    public DateTime DataAvaliacao { get; private set; }

    public int PedidoId { get; private set; }
    public Pedido? Pedido { get; private set; }
    public int ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    public int RestauranteId { get; private set; }
    public Restaurante? Restaurante { get; private set; }

    protected Avaliacao()
    {
    }

    public Avaliacao(Pedido pedido, int nota, string? comentario, DateTime dataAvaliacao)
    {
        if (!pedido.PodeSerAvaliado())
            throw new RegraDeNegocioException("Somente pedidos ENTREGUES e ainda não avaliados podem ser avaliados.");

        ValidarNota(nota);
        Nota = nota;
        Comentario = Guarda.TextoOpcional(comentario, "Comentário", TamanhoMaximoComentario);
        DataAvaliacao = dataAvaliacao;
        PedidoId = pedido.Id;
        ClienteId = pedido.ClienteId;
        RestauranteId = pedido.RestauranteId;
    }

    public static void ValidarNota(int nota)
    {
        if (nota is < NotaMinima or > NotaMaxima)
            throw new RegraDeNegocioException($"A nota deve estar entre {NotaMinima} e {NotaMaxima} estrelas.");
    }
}
