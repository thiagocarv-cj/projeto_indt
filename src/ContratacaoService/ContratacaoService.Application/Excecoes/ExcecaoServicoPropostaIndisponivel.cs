namespace ContratacaoService.Application.Excecoes;

public class ExcecaoServicoPropostaIndisponivel : Exception
{
    public ExcecaoServicoPropostaIndisponivel(string message, Exception? inner = null) : base(message, inner)
    {
    }
}
