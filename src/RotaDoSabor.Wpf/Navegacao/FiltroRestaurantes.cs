namespace RotaDoSabor.Wpf.Navegacao;

/// <summary>Parâmetro da tela de restaurantes (pesquisa e/ou categoria; nulo = todos).</summary>
public sealed record FiltroRestaurantes(string? TermoBusca, int? CategoriaId);
