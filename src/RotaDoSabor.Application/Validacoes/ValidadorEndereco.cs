namespace RotaDoSabor.Application.Validacoes;

using RotaDoSabor.Application.Dtos;

public static class ValidadorEndereco
{
    public static IReadOnlyList<string> Validar(SalvarEnderecoDto dto)
    {
        var erros = new List<string>();

        if (RegrasValidacao.ApenasDigitos(dto.Cep).Length != 8)
            erros.Add("CEP deve ter 8 dígitos.");
        if (RegrasValidacao.Vazio(dto.Logradouro))
            erros.Add("Informe o logradouro (rua/avenida).");
        if (RegrasValidacao.Vazio(dto.Numero))
            erros.Add("Informe o número.");
        if (RegrasValidacao.Vazio(dto.Bairro))
            erros.Add("Informe o bairro.");
        if (RegrasValidacao.Vazio(dto.Cidade))
            erros.Add("Informe a cidade.");

        return erros;
    }
}
