namespace RotaDoSabor.Application.Comum;

public sealed class RelogioSistema : IRelogio
{
    public DateTime Agora => DateTime.Now;
}
