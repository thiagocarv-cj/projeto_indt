using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.Portas.Saida;

public interface IClienteServicoProposta
{
    Task<RespostaStatusPropostaCompartilhada?> ObterStatusAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> AlterarStatusAsync(Guid propostaId, SolicitacaoAlterarStatusPropostaCompartilhada solicitacao, CancellationToken cancellationToken = default);
}
