namespace RotaDoSabor.Application.Excecoes;

using RotaDoSabor.Domain.Excecoes;

public class NaoEncontradoException : RegraDeNegocioException
{
    public NaoEncontradoException(string recurso) : base($"{recurso} não encontrado(a).")
    {
    }
}
