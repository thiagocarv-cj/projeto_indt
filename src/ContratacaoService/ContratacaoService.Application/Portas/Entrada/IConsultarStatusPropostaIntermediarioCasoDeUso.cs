using Compartilhado.Contratos.Propostas;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IConsultarStatusPropostaIntermediarioCasoDeUso
{
    Task<RespostaStatusPropostaCompartilhada?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
