namespace RotaDoSabor.Common;

/// <summary>
/// Resultado de uma operação (sucesso ou falha com mensagem para o usuário).
/// Clean Code: evita usar exceções para fluxos esperados (ex.: senha incorreta).
/// </summary>
public class Resultado
{
    public bool Sucesso { get; }
    public string Mensagem { get; }

    protected Resultado(bool sucesso, string mensagem)
    {
        Sucesso = sucesso;
        Mensagem = mensagem;
    }

    public static Resultado Ok() => new(true, string.Empty);

    public static Resultado Falha(string mensagem) => new(false, mensagem);
}

/// <summary>Resultado que, em caso de sucesso, também carrega um valor.</summary>
public sealed class Resultado<T> : Resultado
{
    public T? Valor { get; }

    private Resultado(bool sucesso, string mensagem, T? valor) : base(sucesso, mensagem)
    {
        Valor = valor;
    }

    public static Resultado<T> Ok(T valor) => new(true, string.Empty, valor);

    public new static Resultado<T> Falha(string mensagem) => new(false, mensagem, default);
}
