namespace Shared.Contracts.Eventos;

public record EventoPropostaAprovada(
    Guid PropostaId,
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura,
    DateTime DataAprovacao);

public record EventoPropostaRejeitada(
    Guid PropostaId,
    string? Motivo,
    DateTime DataRejeicao);

public record EventoPropostaPendente(
    Guid PropostaId,
    string Observacao,
    DateTime DataPendencia);
