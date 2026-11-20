namespace RotaDoSabor.Application.Dtos;

public sealed record CadastroClienteDto(
    string Nome,
    string Email,
    string NomeUsuario,
    string Telefone,
    string Senha,
    string ConfirmacaoSenha);

public sealed record PerfilDto(string Nome, string Email, string NomeUsuario, string Telefone, DateTime DataCadastro);

public sealed record AtualizarPerfilDto(string Nome, string Telefone);
