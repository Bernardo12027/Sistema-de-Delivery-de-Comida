namespace RotaDoSabor.Application.Dtos;

public sealed record EnderecoDto(
    int Id,
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    bool Padrao,
    string Descricao)
{
    public string DescricaoComPadrao => Padrao ? $"[PADRÃO] {Descricao}" : Descricao;
}

/// <summary>Id nulo = novo endereço; Id preenchido = edição.</summary>
public sealed record SalvarEnderecoDto(
    int? Id,
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    bool TornarPadrao);
