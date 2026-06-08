namespace Compartilhado.Contratos.Propostas;

/// <summary>Status atual de uma proposta retornado pelo PropostaService.</summary>
public record RespostaStatusPropostaCompartilhada(
    Guid PropostaId,
    StatusPropostaCompartilhado Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);
