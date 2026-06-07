using PropostaService.Application.DTOs;

namespace PropostaService.Application.Portas.Entrada;

public interface IAlterarStatusPropostaCasoDeUso
{
    Task<RespostaStatusProposta> ExecutarAsync(Guid id, SolicitacaoAlterarStatusProposta solicitacao, CancellationToken cancellationToken = default);
}
