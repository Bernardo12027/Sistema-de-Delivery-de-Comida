namespace RotaDoSabor.Application.Sessao;

using RotaDoSabor.Domain.Entidades;
using RotaDoSabor.Domain.Excecoes;

public sealed class SessaoUsuario : ISessaoUsuario
{
    public bool EstaAutenticado => ClienteId.HasValue;
    public int? ClienteId { get; private set; }
    public string NomeCliente { get; private set; } = string.Empty;
    public Carrinho Carrinho { get; private set; } = new();

    public void Iniciar(int clienteId, string nomeCliente)
    {
        ClienteId = clienteId;
        NomeCliente = nomeCliente;
        Carrinho = new Carrinho();
    }

    public void AtualizarNome(string nomeCliente) => NomeCliente = nomeCliente;

    public void Encerrar()
    {
        ClienteId = null;
        NomeCliente = string.Empty;
        Carrinho = new Carrinho();
    }

    public int ObterClienteIdObrigatorio() =>
        ClienteId ?? throw new RegraDeNegocioException("Sessão expirada. Faça login novamente.");
}
