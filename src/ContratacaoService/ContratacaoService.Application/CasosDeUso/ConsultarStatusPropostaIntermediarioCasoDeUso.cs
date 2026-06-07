using ContratacaoService.Application.Portas.Entrada;
using ContratacaoService.Application.Portas.Saida;
using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.CasosDeUso;

public class ConsultarStatusPropostaIntermediarioCasoDeUso(IClienteServicoProposta cliente) : IConsultarStatusPropostaIntermediarioCasoDeUso
{
    public Task<RespostaStatusPropostaCompartilhada?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default)
        => cliente.ObterStatusAsync(propostaId, cancellationToken);
}
