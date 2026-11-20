namespace RotaDoSabor.Application.Dtos;

using RotaDoSabor.Domain.Enums;

public sealed record CartaoDto(int Id, string Apelido, string Bandeira, string UltimosDigitos, TipoCartao Tipo, string Descricao);

/// <summary>
/// Dados digitados na tela. Número completo, nome, validade e CVV são usados SOMENTE para validação
/// e descartados; nada disso é gravado no banco.
/// </summary>
public sealed record NovoCartaoDto(TipoCartao Tipo, string Numero, string NomeTitular, string Validade, string Cvv, string Apelido);
