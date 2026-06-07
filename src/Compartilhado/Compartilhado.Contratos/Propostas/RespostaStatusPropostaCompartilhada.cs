namespace Compartilhado.Contratos.Propostas;

public record RespostaStatusPropostaCompartilhada(
    Guid PropostaId,
    StatusPropostaCompartilhado Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);
