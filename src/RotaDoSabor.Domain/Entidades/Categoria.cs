namespace RotaDoSabor.Domain.Entidades;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Categoria usada para classificar restaurantes e produtos (Hambúrguer, Pizza, ...).</summary>
public class Categoria
{
    public const int TamanhoMaximoNome = 50;

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;

    public ICollection<Restaurante> Restaurantes { get; private set; } = new List<Restaurante>();
    public ICollection<Produto> Produtos { get; private set; } = new List<Produto>();

    protected Categoria()
    {
    }

    public Categoria(string nome)
    {
        Nome = Guarda.TextoObrigatorio(nome, "Nome da categoria", TamanhoMaximoNome);
    }
}
