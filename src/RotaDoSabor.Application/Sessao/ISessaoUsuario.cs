namespace RotaDoSabor.Application.Sessao;

using RotaDoSabor.Domain.Entidades;

/// <summary>Dados do cliente autenticado e carrinho atual (vivem enquanto o aplicativo estiver aberto).</summary>
public interface ISessaoUsuario
{
    bool EstaAutenticado { get; }
    int? ClienteId { get; }
    string NomeCliente { get; }
    Carrinho Carrinho { get; }

    void Iniciar(int clienteId, string nomeCliente);
    void AtualizarNome(string nomeCliente);
    void Encerrar();

    /// <summary>Retorna o Id do cliente logado ou lança exceção se ninguém estiver autenticado.</summary>
    int ObterClienteIdObrigatorio();
}
