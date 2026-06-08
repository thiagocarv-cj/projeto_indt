using PropostaService.Domain.Enums;

namespace PropostaService.Application.DTOs;

/// <summary>Dados para criação de uma nova proposta de seguro.</summary>
/// <param name="NomeSegurado">Nome completo do segurado (1–200 caracteres).</param>
/// <param name="Cpf">CPF do segurado, somente dígitos ou formatado (máx. 14 caracteres).</param>
/// <param name="ValorCobertura">Valor da cobertura em reais; deve ser maior que zero.</param>
public record SolicitacaoCriarProposta(
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura);

/// <summary>Alteração de status de uma proposta existente.</summary>
/// <param name="Status">Novo status. Valores: <c>EmAnalise</c>, <c>Aprovada</c>, <c>Rejeitada</c>, <c>Pendencias</c>.</param>
/// <param name="Observacao">Obrigatória quando <paramref name="Status"/> é <c>Pendencias</c> (mín. 10 caracteres). Opcional para <c>Rejeitada</c>.</param>
public record SolicitacaoAlterarStatusProposta(StatusProposta Status, string? Observacao);

/// <summary>Representação completa de uma proposta.</summary>
public record RespostaProposta(
    Guid Id,
    string NomeSegurado,
    string Cpf,
    decimal ValorCobertura,
    StatusProposta Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);

/// <summary>Resumo do status atual de uma proposta.</summary>
public record RespostaStatusProposta(
    Guid PropostaId,
    StatusProposta Status,
    string? Observacao,
    DateTime DataCriacao,
    DateTime DataAtualizacao);
