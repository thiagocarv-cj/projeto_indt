namespace ContratacaoService.Domain.Entities;

public class Contratacao
{
    public Guid Id { get; private set; }
    public Guid PropostaId { get; private set; }
    public string NomeSegurado { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public decimal ValorCobertura { get; private set; }
    public DateTime DataContratacao { get; private set; }

    private Contratacao()
    {
    }

    public static Contratacao Criar(
        Guid propostaId,
        string nomeSegurado,
        string cpf,
        decimal valorCobertura,
        DateTime dataContratacao)
    {
        return new Contratacao
        {
            Id = Guid.NewGuid(),
            PropostaId = propostaId,
            NomeSegurado = nomeSegurado,
            Cpf = cpf,
            ValorCobertura = valorCobertura,
            DataContratacao = dataContratacao
        };
    }
}
