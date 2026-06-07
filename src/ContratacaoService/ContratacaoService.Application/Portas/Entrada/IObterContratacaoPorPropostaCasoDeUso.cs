using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IObterContratacaoPorPropostaCasoDeUso
{
    Task<RespostaContratacao?> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
