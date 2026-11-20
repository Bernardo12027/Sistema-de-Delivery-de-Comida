namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Excecoes;

public interface ICupomService
{
    /// <summary>Confere se o cupom existe e está válido, retornando o desconto calculado para o subtotal.</summary>
    Task<CupomAplicadoDto> ValidarAsync(string codigo, decimal subtotal);

    /// <summary>Usado na finalização do pedido (retorna a entidade validada).</summary>
    Task<Cupom> ObterValidoAsync(string codigo);
}

public sealed class CupomService : ICupomService
{
    private readonly ICupomRepository _cupons;
    private readonly IRelogio _relogio;

    public CupomService(ICupomRepository cupons, IRelogio relogio)
    {
        _cupons = cupons;
        _relogio = relogio;
    }

    public async Task<CupomAplicadoDto> ValidarAsync(string codigo, decimal subtotal)
    {
        var cupom = await ObterValidoAsync(codigo);
        var desconto = cupom.CalcularDesconto(subtotal);
        return new CupomAplicadoDto(cupom.Codigo, cupom.Descricao, cupom.PercentualDesconto, desconto, subtotal - desconto);
    }

    public async Task<Cupom> ObterValidoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new RegraDeNegocioException("Informe o código do cupom.");

        var cupom = await _cupons.ObterPorCodigoAsync(Cupom.NormalizarCodigo(codigo))
            ?? throw new RegraDeNegocioException("Cupom não encontrado.");

        if (!cupom.Ativo)
            throw new RegraDeNegocioException("Este cupom está desativado.");
        if (!cupom.EstaValido(_relogio.Agora))
            throw new RegraDeNegocioException($"Cupom expirado em {cupom.DataValidade:dd/MM/yyyy}.");

        return cupom;
    }
}
