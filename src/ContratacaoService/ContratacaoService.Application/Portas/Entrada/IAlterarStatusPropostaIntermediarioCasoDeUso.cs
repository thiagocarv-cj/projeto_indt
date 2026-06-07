using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IAlterarStatusPropostaIntermediarioCasoDeUso
{
    Task<HttpResponseMessage> ExecutarAsync(Guid propostaId, SolicitacaoAlterarStatusPropostaCompartilhada solicitacao, CancellationToken cancellationToken = default);
}
