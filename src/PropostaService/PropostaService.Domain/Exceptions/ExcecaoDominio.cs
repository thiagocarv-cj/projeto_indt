namespace PropostaService.Domain.Excecoes;

public class ExcecaoDominio : Exception
{
    public ExcecaoDominio(string message) : base(message)
    {
    }
}
