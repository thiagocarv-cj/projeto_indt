namespace Compartilhado.Contratos.Propostas;

/// <summary>Solicitação de alteração de status repassada ao PropostaService.</summary>
/// <param name="Status">Novo status. Valores: <c>EmAnalise</c>, <c>Aprovada</c>, <c>Rejeitada</c>, <c>Pendencias</c>.</param>
/// <param name="Observacao">Obrigatória quando <paramref name="Status"/> é <c>Pendencias</c> (mín. 10 caracteres).</param>
public record SolicitacaoAlterarStatusPropostaCompartilhada(
    StatusPropostaCompartilhado Status,
    string? Observacao);
