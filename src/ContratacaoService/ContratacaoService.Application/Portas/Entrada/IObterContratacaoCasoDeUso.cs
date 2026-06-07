using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Portas.Entrada;

public interface IObterContratacaoCasoDeUso
{
    Task<RespostaContratacao?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
