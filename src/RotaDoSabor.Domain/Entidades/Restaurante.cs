namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Restaurante parceiro. A média de avaliação é recalculada a cada nova avaliação salva
/// (média ponderada entre a base inicial do seed e as avaliações registradas no sistema).
/// </summary>
public class Restaurante
{
    public const int TamanhoMaximoNome = 100;
    public const int TamanhoMaximoEndereco = 200;
    public const int TamanhoMaximoTelefone = 20;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Endereco { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public decimal AvaliacaoMedia { get; private set; }
    public int QuantidadeAvaliacoes { get; private set; }
    public int TempoEntregaMinutos { get; private set; }

    public int CategoriaId { get; private set; }
    public Categoria? Categoria { get; private set; }

    public ICollection<Produto> Produtos { get; private set; } = new List<Produto>();
    public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();
    public ICollection<Avaliacao> Avaliacoes { get; private set; } = new List<Avaliacao>();

    protected Restaurante()
    {
    }

    public Restaurante(string nome, string endereco, string telefone, int categoriaId, int tempoEntregaMinutos,
        decimal avaliacaoMediaInicial = 0m, int quantidadeAvaliacoesInicial = 0)
    {
        Nome = Guarda.TextoObrigatorio(nome, "Nome do restaurante", TamanhoMaximoNome);
        Endereco = Guarda.TextoObrigatorio(endereco, "Endereço do restaurante", TamanhoMaximoEndereco);
        Telefone = Guarda.TextoObrigatorio(telefone, "Telefone do restaurante", TamanhoMaximoTelefone);
        CategoriaId = Guarda.IdValido(categoriaId, "Categoria");
        TempoEntregaMinutos = tempoEntregaMinutos > 0 ? tempoEntregaMinutos : throw new RegraDeNegocioException("Tempo de entrega inválido.");

        if (avaliacaoMediaInicial is < 0 or > Avaliacao.NotaMaxima || quantidadeAvaliacoesInicial < 0)
            throw new RegraDeNegocioException("Avaliação inicial inválida.");

        AvaliacaoMedia = avaliacaoMediaInicial;
        QuantidadeAvaliacoes = quantidadeAvaliacoesInicial;
    }

    /// <summary>Atualiza a média com a nova nota: média = (média * qtd + nota) / (qtd + 1).</summary>
    public void RegistrarAvaliacao(int nota)
    {
        Avaliacao.ValidarNota(nota);

        var somaAtual = AvaliacaoMedia * QuantidadeAvaliacoes;
        QuantidadeAvaliacoes++;
        AvaliacaoMedia = Math.Round((somaAtual + nota) / QuantidadeAvaliacoes, 2, MidpointRounding.AwayFromZero);
    }
}
