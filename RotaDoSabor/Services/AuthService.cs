using RotaDoSabor.Common;
using RotaDoSabor.Models;
using RotaDoSabor.Repositories;

namespace RotaDoSabor.Services;

/// <summary>
/// Regras de negócio de login e cadastro.
/// SOLID (D): recebe repositório e hasher por interface (injeção de dependência).
/// </summary>
public class AuthService : IAuthService
{
    // Mensagem única para "usuário não existe" e "senha errada": não revela quais contas existem.
    private const string CredenciaisInvalidas = "Usuário/e-mail ou senha inválidos.";

    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _hasher = hasher;
    }

    public async Task<Resultado<Usuario>> EntrarAsync(string identificador, string senha)
    {
        identificador = identificador.Trim();
        if (identificador.Length == 0 || senha.Length == 0)
            return Resultado<Usuario>.Falha("Informe o usuário/e-mail e a senha.");

        var usuario = await _usuarios.ObterPorNomeOuEmailAsync(identificador);
        if (usuario is null)
            return Resultado<Usuario>.Falha(CredenciaisInvalidas);

        // O hash é propositalmente lento; roda fora da thread da interface para a janela não travar.
        var senhaCorreta = await Task.Run(() => _hasher.Verificar(senha, usuario.SenhaHash));

        return senhaCorreta
            ? Resultado<Usuario>.Ok(usuario)
            : Resultado<Usuario>.Falha(CredenciaisInvalidas);
    }

    public async Task<Resultado> CadastrarAsync(DadosCadastro dados)
    {
        dados = dados with { NomeUsuario = dados.NomeUsuario.Trim(), Email = dados.Email.Trim() };

        var erro = ValidadorCadastro.Validar(dados);
        if (erro is not null)
            return Resultado.Falha(erro);

        if (await _usuarios.ExisteNomeUsuarioAsync(dados.NomeUsuario))
            return Resultado.Falha("Este nome de usuário já está em uso.");

        if (await _usuarios.ExisteEmailAsync(dados.Email))
            return Resultado.Falha("Este e-mail já está cadastrado.");

        var hash = await Task.Run(() => _hasher.Hash(dados.Senha));

        await _usuarios.AdicionarAsync(new Usuario
        {
            NomeUsuario = dados.NomeUsuario,
            Email = dados.Email,
            SenhaHash = hash
        });

        return Resultado.Ok();
    }
}
