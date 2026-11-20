namespace RotaDoSabor.Domain.Excecoes;

/// <summary>
/// Lançada quando uma regra de negócio do domínio é violada (ex.: cancelar pedido já em preparo).
/// A camada de aplicação converte esta exceção em mensagem amigável para a tela.
/// </summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}
