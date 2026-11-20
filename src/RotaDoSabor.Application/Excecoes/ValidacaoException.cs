namespace RotaDoSabor.Application.Excecoes;

using RotaDoSabor.Domain.Excecoes;

/// <summary>Erros de preenchimento de formulário (um ou mais campos inválidos).</summary>
public class ValidacaoException : RegraDeNegocioException
{
    public IReadOnlyList<string> Erros { get; }

    public ValidacaoException(IReadOnlyList<string> erros)
        : base(string.Join(Environment.NewLine, erros))
    {
        Erros = erros;
    }

    public static void LancarSeHouverErros(IReadOnlyList<string> erros)
    {
        if (erros.Count > 0)
            throw new ValidacaoException(erros);
    }
}
