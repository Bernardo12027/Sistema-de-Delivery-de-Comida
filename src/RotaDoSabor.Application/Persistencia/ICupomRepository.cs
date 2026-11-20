namespace RotaDoSabor.Application.Persistencia;

using RotaDoSabor.Domain.Entidades;

public interface ICupomRepository
{
    Task<Cupom?> ObterPorCodigoAsync(string codigo);
}
