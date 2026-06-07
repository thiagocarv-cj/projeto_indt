using PropostaService.Domain.Enums;

namespace PropostaService.Application.DTOs;

public record SolicitacaoCriarProposta(string NomeSegurado, string Cpf, decimal ValorCobertura);

public record SolicitacaoAlterarStatusProposta(StatusProposta Status, string? Observacao);

public record RespostaProposta(
    Guid Id,
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura,
    StatusProposta Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);

public record RespostaStatusProposta(
    Guid PropostaId,
    StatusProposta Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);
