using RotaDoSabor.Models;

namespace RotaDoSabor.Services;

/// <summary>Guarda quem está logado enquanto o aplicativo está aberto.</summary>
public interface ISessaoUsuario
{
    Usuario? UsuarioLogado { get; }

    bool EstaLogado { get; }

    void Iniciar(Usuario usuario);

    void Encerrar();
}
