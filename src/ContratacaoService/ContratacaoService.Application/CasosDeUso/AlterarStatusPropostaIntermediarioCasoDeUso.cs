using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.CasosDeUso;

public class AlterarStatusPropostaIntermediarioCasoDeUso(IClienteServicoProposta cliente) : IAlterarStatusPropostaIntermediarioCasoDeUso
{
    public Task<HttpResponseMessage> ExecutarAsync(
        Guid propostaId,
        SolicitacaoAlterarStatusPropostaCompartilhada solicitacao,
        CancellationToken cancellationToken = default)
        => cliente.AlterarStatusAsync(propostaId, solicitacao, cancellationToken);
}
