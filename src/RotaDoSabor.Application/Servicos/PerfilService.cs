namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Sessao;
using RotaDoSabor.Application.Validacoes;

/// <summary>NOVA FUNCIONALIDADE: editar perfil.</summary>
public interface IPerfilService
{
    Task<PerfilDto> ObterAsync(int clienteId);
    Task AtualizarAsync(int clienteId, AtualizarPerfilDto dados);
}

public sealed class PerfilService : IPerfilService
{
    private readonly IClienteRepository _clientes;
    private readonly ISessaoUsuario _sessao;

    public PerfilService(IClienteRepository clientes, ISessaoUsuario sessao)
    {
        _clientes = clientes;
        _sessao = sessao;
    }

    public async Task<PerfilDto> ObterAsync(int clienteId)
    {
        var cliente = await _clientes.ObterPorIdAsync(clienteId) ?? throw new NaoEncontradoException("Cliente");
        return new PerfilDto(cliente.Nome, cliente.Email, cliente.NomeUsuario, cliente.Telefone, cliente.DataCadastro);
    }

    public async Task AtualizarAsync(int clienteId, AtualizarPerfilDto dados)
    {
        ValidacaoException.LancarSeHouverErros(ValidadorPerfil.Validar(dados));

        var cliente = await _clientes.ObterPorIdAsync(clienteId) ?? throw new NaoEncontradoException("Cliente");
        cliente.AtualizarPerfil(dados.Nome, RegrasValidacao.ApenasDigitos(dados.Telefone));
        await _clientes.AtualizarAsync(cliente);
        _sessao.AtualizarNome(cliente.Nome);
    }
}
