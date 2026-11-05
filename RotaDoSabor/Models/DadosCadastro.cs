namespace RotaDoSabor.Models;

/// <summary>Dados digitados na tela de cadastro (ainda não validados).</summary>
public sealed record DadosCadastro(
    string NomeUsuario,
    string Email,
    string Senha,
    string ConfirmacaoSenha);
