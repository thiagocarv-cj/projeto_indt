using ContratacaoService.Application.DTOs;
using Shared.Contracts.Eventos;
using Shared.Contracts.Propostas;

namespace ContratacaoService.Application.UseCases;

public interface IIngerirPropostaAprovadaCasoDeUso
{
    Task ExecutarAsync(EventoPropostaAprovada evento, CancellationToken cancellationToken = default);
}

public interface IConsultarStatusPropostaIntermediarioCasoDeUso
{
    Task<RespostaStatusPropostaCompartilhada?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default);
}

public interface IAlterarStatusPropostaIntermediarioCasoDeUso
{
    Task<HttpResponseMessage> ExecutarAsync(Guid propostaId, SolicitacaoAlterarStatusPropostaCompartilhada solicitacao, CancellationToken cancellationToken = default);
}

public interface IListarContratacoesCasoDeUso
{
    Task<ResultadoPaginado<RespostaContratacao>> ExecutarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
}

public interface IObterContratacaoCasoDeUso
{
    Task<RespostaContratacao?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IObterContratacaoPorPropostaCasoDeUso
{
    Task<RespostaContratacao?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
