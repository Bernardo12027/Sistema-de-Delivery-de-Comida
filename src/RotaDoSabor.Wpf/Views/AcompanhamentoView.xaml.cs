namespace RotaDoSabor.Wpf.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using RotaDoSabor.Application.Comum;
using RotaDoSabor.Application.Dtos;
using RotaDoSabor.Wpf.Controllers;
using RotaDoSabor.Wpf.Navegacao;

/// <summary>Detalhe e acompanhamento do pedido (status persistido no banco pela simulação).</summary>
public partial class AcompanhamentoView : UserControl, ITelaComParametro
{
    private readonly PedidoController _controller;
    private readonly DispatcherTimer _atualizacaoAutomatica = new() { Interval = TimeSpan.FromSeconds(3) };
    private int _pedidoId;
    private bool _atualizando;

    public AcompanhamentoView(PedidoController controller)
    {
        InitializeComponent();
        _controller = controller;
        _atualizacaoAutomatica.Tick += async (_, _) => await AtualizarAsync();
        Unloaded += (_, _) => _atualizacaoAutomatica.Stop();
    }

    public async void AoNavegar(object? parametro)
    {
        if (parametro is not int pedidoId)
            return;

        _pedidoId = pedidoId;
        await AtualizarAsync();
        _atualizacaoAutomatica.Start();
    }

    private async Task AtualizarAsync()
    {
        if (_atualizando)
            return;

        _atualizando = true;
        try
        {
            var pedido = await _controller.CarregarAsync(_pedidoId);
            if (pedido is null)
            {
                _atualizacaoAutomatica.Stop();
                return;
            }
            Exibir(pedido);
        }
        finally
        {
            _atualizando = false;
        }
    }

    private void Exibir(DetalhePedidoDto pedido)
    {
        TxtTitulo.Text = $"Pedido #{pedido.Id}";
        TxtRestaurante.Text = pedido.NomeRestaurante;
        TxtData.Text = $"Realizado em {Formatacao.DataHora(pedido.DataHora)}";
        TxtStatus.Text = pedido.StatusTexto;
        BarraProgresso.Value = pedido.Progresso;
        TxtTempo.Text = $"Tempo estimado (preparo + entrega): {pedido.TempoEstimadoMinutos} min — previsão de entrega às {Formatacao.Hora(pedido.PrevisaoEntrega)}";
        TxtEndereco.Text = $"Entrega em: {pedido.EnderecoEntrega}";
        TxtPagamento.Text = $"Pagamento: {pedido.PagamentoTexto}" + (pedido.CodigoCupom is null ? string.Empty : $" · Cupom: {pedido.CodigoCupom}");
        TxtValores.Text = $"Subtotal {Formatacao.Moeda(pedido.Subtotal)}  −  Desconto {Formatacao.Moeda(pedido.Desconto)}  +  Entrega grátis  =  Total {Formatacao.Moeda(pedido.ValorTotal)}";
        DgItens.ItemsSource = pedido.Itens;

        var emAndamento = pedido.Progresso is > 0 and < 100;
        BtnCancelar.IsEnabled = pedido.PodeCancelar;
        BtnAvaliar.IsEnabled = pedido.PodeAvaliar;
        BtnAvancar.IsEnabled = emAndamento;

        if (!emAndamento)
            _atualizacaoAutomatica.Stop();
    }

    private async void BtnAtualizar_Click(object sender, RoutedEventArgs e) => await AtualizarAsync();

    private async void BtnAvancar_Click(object sender, RoutedEventArgs e)
    {
        if (await _controller.AvancarStatusAsync(_pedidoId))
            await AtualizarAsync();
    }

    private async void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        await _controller.CancelarAsync(_pedidoId);
        await AtualizarAsync();
    }

    private void BtnAvaliar_Click(object sender, RoutedEventArgs e) => _controller.Avaliar(_pedidoId);

    private async void BtnRepetir_Click(object sender, RoutedEventArgs e) => await _controller.RepetirAsync(_pedidoId);

    private void BtnHistorico_Click(object sender, RoutedEventArgs e) => _controller.AbrirHistorico();

    private void BtnInicio_Click(object sender, RoutedEventArgs e) => _controller.IrParaHome();
}
