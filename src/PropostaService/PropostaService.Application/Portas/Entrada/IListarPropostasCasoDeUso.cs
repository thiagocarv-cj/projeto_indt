using PropostaService.Application.DTOs;

namespace PropostaService.Application.Portas.Entrada;

public interface IListarPropostasCasoDeUso
{
    Task<IReadOnlyList<RespostaProposta>> ExecutarAsync(string? status, CancellationToken cancellationToken = default);
}
