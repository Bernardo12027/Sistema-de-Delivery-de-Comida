using RotaDoSabor.Models;

namespace RotaDoSabor.Services;

public class SessaoUsuario : ISessaoUsuario
{
    public Usuario? UsuarioLogado { get; private set; }

    public bool EstaLogado => UsuarioLogado is not null;

    public void Iniciar(Usuario usuario) => UsuarioLogado = usuario;

    public void Encerrar() => UsuarioLogado = null;
}
