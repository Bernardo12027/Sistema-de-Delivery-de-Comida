namespace RotaDoSabor.Application.Servicos;

using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Application.Excecoes;
using RotaDoSabor.Application.Persistencia;
using RotaDoSabor.Application.Validacoes;
using RotaDoSabor.Domain.Entidades;

public interface IEnderecoService
{
    Task<IReadOnlyList<EnderecoDto>> ListarAsync(int clienteId);
    Task<int> SalvarAsync(int clienteId, SalvarEnderecoDto dados);

    /// <summary>Exclui o endereço. Se ele já foi usado em pedidos, é apenas inativado (histórico preservado).</summary>
    Task<string> ExcluirAsync(int clienteId, int enderecoId);

    /// <summary>NOVA FUNCIONALIDADE: definir endereço padrão (apenas um por cliente).</summary>
    Task DefinirPadraoAsync(int clienteId, int enderecoId);
}

public sealed class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _enderecos;

    public EnderecoService(IEnderecoRepository enderecos)
    {
        _enderecos = enderecos;
    }

    public async Task<IReadOnlyList<EnderecoDto>> ListarAsync(int clienteId)
    {
        var enderecos = await _enderecos.ListarAtivosPorClienteAsync(clienteId);
        return enderecos
            .OrderByDescending(endereco => endereco.Padrao)
            .ThenBy(endereco => endereco.Logradouro)
            .Select(endereco => endereco.ParaDto())
            .ToList();
    }

    public async Task<int> SalvarAsync(int clienteId, SalvarEnderecoDto dados)
    {
        ValidacaoException.LancarSeHouverErros(ValidadorEndereco.Validar(dados));

        var enderecos = (await _enderecos.ListarAtivosPorClienteAsync(clienteId)).ToList();
        return dados.Id is null
            ? await CriarAsync(clienteId, dados, enderecos)
            : await EditarAsync(clienteId, dados, enderecos);
    }

    public async Task<string> ExcluirAsync(int clienteId, int enderecoId)
    {
        var enderecos = (await _enderecos.ListarAtivosPorClienteAsync(clienteId)).ToList();
        var endereco = BuscarDoCliente(enderecos, enderecoId);
        var eraPadrao = endereco.Padrao;
        string mensagem;

        if (await _enderecos.PossuiPedidosAsync(enderecoId))
        {
            endereco.Inativar();
            await _enderecos.AtualizarAsync([endereco]);
            mensagem = "Endereço removido da sua lista. Ele continua vinculado aos pedidos antigos (histórico preservado).";
        }
        else
        {
            await _enderecos.RemoverAsync(endereco);
            mensagem = "Endereço excluído.";
        }

        enderecos.Remove(endereco);
        if (eraPadrao && enderecos.Count > 0)
        {
            MarcarComoUnicoPadrao(enderecos, enderecos[0]);
            await _enderecos.AtualizarAsync(enderecos);
        }

        return mensagem;
    }

    public async Task DefinirPadraoAsync(int clienteId, int enderecoId)
    {
        var enderecos = await _enderecos.ListarAtivosPorClienteAsync(clienteId);
        var escolhido = BuscarDoCliente(enderecos, enderecoId);
        MarcarComoUnicoPadrao(enderecos, escolhido);
        await _enderecos.AtualizarAsync(enderecos);
    }

    private async Task<int> CriarAsync(int clienteId, SalvarEnderecoDto dados, List<Endereco> existentes)
    {
        var novo = new Endereco(clienteId, dados.Cep, dados.Logradouro, dados.Numero, dados.Complemento, dados.Bairro, dados.Cidade);

        // O primeiro endereço do cliente vira padrão automaticamente.
        if (dados.TornarPadrao || existentes.Count == 0)
        {
            MarcarComoUnicoPadrao(existentes, novo);
            await _enderecos.AtualizarAsync(existentes);
        }

        await _enderecos.AdicionarAsync(novo);
        return novo.Id;
    }

    private async Task<int> EditarAsync(int clienteId, SalvarEnderecoDto dados, List<Endereco> existentes)
    {
        var endereco = BuscarDoCliente(existentes, dados.Id!.Value);

        // Endereço já usado em pedido: cria uma nova versão e inativa a antiga para não alterar o histórico.
        if (await _enderecos.PossuiPedidosAsync(endereco.Id))
        {
            var deveSerPadrao = endereco.Padrao || dados.TornarPadrao;
            endereco.Inativar();
            await _enderecos.AtualizarAsync([endereco]);
            existentes.Remove(endereco);
            return await CriarAsync(clienteId, dados with { Id = null, TornarPadrao = deveSerPadrao }, existentes);
        }

        endereco.Atualizar(dados.Cep, dados.Logradouro, dados.Numero, dados.Complemento, dados.Bairro, dados.Cidade);
        if (dados.TornarPadrao)
            MarcarComoUnicoPadrao(existentes, endereco);

        await _enderecos.AtualizarAsync(existentes);
        return endereco.Id;
    }

    private static Endereco BuscarDoCliente(IEnumerable<Endereco> enderecosDoCliente, int enderecoId) =>
        enderecosDoCliente.FirstOrDefault(endereco => endereco.Id == enderecoId)
        ?? throw new NaoEncontradoException("Endereço");

    private static void MarcarComoUnicoPadrao(IEnumerable<Endereco> enderecos, Endereco escolhido)
    {
        foreach (var endereco in enderecos)
            endereco.RemoverPadrao();
        escolhido.DefinirComoPadrao();
    }
}
