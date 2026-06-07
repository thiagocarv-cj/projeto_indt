using PropostaService.Domain.Entities;

namespace PropostaService.Application.Portas.Saida;

public interface IPublicadorEventosProposta
{
    Task PublicarStatusAlteradoAsync(Proposta proposta, CancellationToken cancellationToken = default);
}
