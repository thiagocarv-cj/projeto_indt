namespace Compartilhado.Contratos.Propostas;

public record SolicitacaoAlterarStatusPropostaCompartilhada(
    StatusPropostaCompartilhado Status,
    string? Observacao);
