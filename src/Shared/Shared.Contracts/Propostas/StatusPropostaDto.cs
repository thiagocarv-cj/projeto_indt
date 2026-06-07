namespace Shared.Contracts.Propostas;

public enum StatusPropostaCompartilhado
{
    EmAnalise,
    Aprovada,
    Rejeitada,
    Pendencias
}

public record SolicitacaoAlterarStatusPropostaCompartilhada(
    StatusPropostaCompartilhado Status,
    string? Observacao);

public record RespostaStatusPropostaCompartilhada(
    Guid PropostaId,
    StatusPropostaCompartilhado Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);
