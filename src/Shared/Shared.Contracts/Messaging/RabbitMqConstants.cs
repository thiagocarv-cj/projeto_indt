namespace Shared.Contracts.Mensageria;

public static class ConstantesRabbitMq
{
    public const string NomeExchange = "proposta.events";
    public const string FilaPropostasAprovadas = "propostas.aprovadas";
    public const string FilaPropostaRejeitada = "proposta.rejeitada";
    public const string FilaPropostaPendencias = "proposta.pendencias";

    public const string ChaveRoteamentoAprovada = "proposta.aprovada";
    public const string ChaveRoteamentoRejeitada = "proposta.rejeitada";
    public const string ChaveRoteamentoPendencias = "proposta.pendencias";
}
