using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Application.Portas.Saida;

public interface IRepositorioProposta
{
    Task AdicionarAsync(Proposta proposta, CancellationToken cancellationToken = default);
    Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Proposta>> ListarAsync(StatusProposta? status, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}
