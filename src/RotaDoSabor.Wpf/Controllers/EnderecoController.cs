namespace RotaDoSabor.Wpf.Controllers;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Servicos;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Cadastro, edição, exclusão e endereço padrão.</summary>
public sealed class EnderecoController : ControllerBase
{
    private readonly IEnderecoService _enderecos;
    private readonly ISessaoUsuario _sessao;

    public EnderecoController(IEnderecoService enderecos, ISessaoUsuario sessao, IDialogoService dialogo, INavegador navegador)
        : base(dialogo, navegador)
    {
        _enderecos = enderecos;
        _sessao = sessao;
    }

    public async Task<IReadOnlyList<EnderecoDto>> ListarAsync() =>
        await ExecutarAsync(() => _enderecos.ListarAsync(_sessao.ObterClienteIdObrigatorio())) ?? [];

    public async Task<bool> SalvarAsync(SalvarEnderecoDto dados)
    {
        var sucesso = await ExecutarAsync(() => _enderecos.SalvarAsync(_sessao.ObterClienteIdObrigatorio(), dados));
        if (sucesso)
            Dialogo.Informar(dados.Id is null ? "Endereço cadastrado." : "Endereço atualizado.");
        return sucesso;
    }

    public async Task<bool> ExcluirAsync(EnderecoDto endereco)
    {
        if (!Dialogo.Confirmar($"Excluir o endereço:\n{endereco.Descricao}?", "Excluir endereço"))
            return false;

        var mensagem = await ExecutarAsync(() => _enderecos.ExcluirAsync(_sessao.ObterClienteIdObrigatorio(), endereco.Id));
        if (mensagem is not null)
            Dialogo.Informar(mensagem);
        return mensagem is not null;
    }

    /// <summary>NOVA FUNCIONALIDADE: definir endereço padrão.</summary>
    public Task<bool> DefinirPadraoAsync(int enderecoId) =>
        ExecutarAsync(() => _enderecos.DefinirPadraoAsync(_sessao.ObterClienteIdObrigatorio(), enderecoId));
}
