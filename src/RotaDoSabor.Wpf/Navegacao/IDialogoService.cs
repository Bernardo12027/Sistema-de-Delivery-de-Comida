namespace RotaDoSabor.Wpf.Navegacao;

/// <summary>Mensagens e confirmações exibidas ao usuário (implementado com MessageBox).</summary>
public interface IDialogoService
{
    bool Confirmar(string mensagem, string titulo = "Confirmação");
    void Informar(string mensagem);
    void Avisar(string mensagem);
    void Erro(string mensagem);
}
