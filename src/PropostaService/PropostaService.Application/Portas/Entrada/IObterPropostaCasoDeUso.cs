using PropostaService.Application.DTOs;

namespace PropostaService.Application.Portas.Entrada;

public interface IObterPropostaCasoDeUso
{
    Task<RespostaProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
