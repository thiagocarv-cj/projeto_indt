using PropostaService.Application.DTOs;

namespace PropostaService.Application.Portas.Entrada;

public interface IConsultarStatusPropostaCasoDeUso
{
    Task<RespostaStatusProposta?> ExecutarAsync(Guid id, CancellationToken cancellationToken = default);
}
