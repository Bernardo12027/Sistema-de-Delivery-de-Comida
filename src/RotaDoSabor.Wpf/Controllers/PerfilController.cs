namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>NOVA FUNCIONALIDADE: editar perfil (tela "Configurações").</summary>
public sealed class PerfilController : ControllerBase
{
    private readonly IPerfilService _perfil;
    private readonly ISessaoUsuario _sessao;

    public PerfilController(IPerfilService perfil, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _perfil = perfil;
        _sessao = sessao;
    }

    public Task<PerfilDto?> CarregarAsync() => ExecutarAsync(() => _perfil.ObterAsync(_sessao.ObterClienteIdObrigatorio()));

    public async Task<bool> SalvarAsync(string nome, string telefone)
    {
        var sucesso = await ExecutarAsync(() => _perfil.AtualizarAsync(_sessao.ObterClienteIdObrigatorio(), new AtualizarPerfilDto(nome, telefone)));
        if (sucesso)
            Dialogo.Informar("Perfil atualizado.");
        return sucesso;
    }

    public void AbrirEnderecos() => Navegador.Navegar(Tela.Enderecos);
    public void AbrirCartoes() => Navegador.Navegar(Tela.Cartoes);
    public void AbrirHistorico() => Navegador.Navegar(Tela.Historico);
}
